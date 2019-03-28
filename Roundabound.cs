using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.FileProviders;

namespace Roundabound {
    public class LogRotate {
        private IEnumerable<RotationSet> _rotationSets;
        private bool _dryRun;
        public LogRotate(IEnumerable<RotationSet> rotationSets, bool dryRun = false) {
            _rotationSets = rotationSets;
            _dryRun = dryRun;
        }

        public void Rotate() {
            // Todo logging.info('Starting log rotation');

            if (_dryRun) {
                Console.Out.WriteLine("Not gonna bother");
                return;
            }

            Console.Out.WriteLine($"I have {_rotationSets.Count()} rotation sets");

            foreach (var rotationSet in _rotationSets) {
                try {
                    Console.Out.WriteLine($"Rotating begins for {rotationSet.pattern}");
                    var rotator = new Rotator(rotationSet);
                    rotator.Rotate();
                } catch (Exception e) {
                    Console.Error.WriteLine(e);
                }
            }
        }
    }

    public class Rotator {
        private RotationSet _rotationSet;
        private int _archiveAge;
        private int _deleteAge;
        private string _filePath;
        private string _filePattern;
        private string _archiveFilePattern;

        public Rotator(RotationSet rotationSet) {
            _rotationSet = rotationSet;
            _archiveAge = ParseAge(_rotationSet.archive_age);
            _deleteAge = ParseAge(_rotationSet.delete_age);
            _filePattern = Path.GetFileName(_rotationSet.pattern);
            _filePath = Path.GetDirectoryName(_rotationSet.pattern);
            _archiveFilePattern = Path.Combine(_rotationSet.archive_path, _filePattern + ".zip");
        }

        private int ParseAge(int age) {
            return age * 24 * 60 * 60;
        }

        public void Rotate() {
            var now = DateTime.Now;

            //var fileProvider = new PhysicalFileProvider(_filePath);
            foreach (var item in Directory.EnumerateFiles(_filePath, _filePattern)) {
                var age = now - File.GetCreationTime(item);
                Console.Out.WriteLine($"Examining file {item}; age {age} - seconds: {age.TotalSeconds}");
                if (_archiveAge > 0 && age.TotalSeconds > _archiveAge) {
                    Archive(item);
                    Console.Out.WriteLine($"Archived {item}");
                } else if (_deleteAge > 0 && age.TotalSeconds > _deleteAge) {
                    Delete(item);
                    Console.Out.WriteLine($"Deleted {item}");
                }
            }

            foreach (var item in Directory.EnumerateFiles(_rotationSet.archive_path, _filePattern + ".zip")) {
                var age = now - File.GetCreationTime(item);
                if (_deleteAge > 0 && age.TotalSeconds > _deleteAge) {
                    Delete(item);
                    Console.Out.WriteLine($"Deleted {item}");
                }
            }
        }

        private void Archive(string filename) {
            var name = Path.GetFileName(filename);
            var archived_name = "";
            if (!string.IsNullOrWhiteSpace(_rotationSet.archive_path)) {
                archived_name = Path.Combine(_rotationSet.archive_path, name + ".zip");
            } else {
                archived_name = name + ".zip";
            }
            using(FileStream fs = new FileStream(archived_name, FileMode.Create))
            using(ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create)) {
                arch.CreateEntryFromFile(filename, name);
            }
        }

        private void Delete(string filePath) {
            File.Delete(filePath);
        }
    }

    public class RotationSet {
        public string pattern { get; set; }
        public int archive_age { get; set; }
        public string archive_path { get; set; }
        public int delete_age { get; set; }
    }

    public class LogRotateError : Exception {

    }
}
