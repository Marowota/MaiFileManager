using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaiFileManager.Classes
{
    static class MaiIcon
    {
        static List<string> archive = new List<string>()
        {
            "7z",
            "a",
            "ar",
            "bz2",
            "cab",
            "cpio",
            "deb",
            "dmg",
            "egg",
            "gz",
            "iso",
            "jar",
            "lha",
            "mar",
            "pea",
            "rar",
            "rpm",
            "s7z",
            "shar",
            "tar",
            "tbz2",
            "tgz",
            "tlz",
            "war",
            "whl",
            "xpi",
            "zip",
            "zipx",
            "xz",
            "pak"
        };
        static List<string> audio = new List<string>()
        {
            "aac",
            "aiff",
            "ape",
            "au",
            "flac",
            "gsm",
            "it",
            "m3u",
            "m4a",
            "mid",
            "mod",
            "mp3",
            "mpa",
            "pls",
            "ra",
            "s3m",
            "sid",
            "wav",
            "wma",
            "xm"
        };
        static List<string> book = new List<string>()
        {
            "mobi",
            "epub",
            "azw1",
            "azw3",
            "azw4",
            "azw6",
            "azw",
            "cbr",
            "cbz"
        };
        static List<string> code = new List<string>()
        {
            "1.ada",
            "2.ada",
            "ada",
            "adb",
            "ads",
            "asm",
            "bas",
            "bash",
            "bat",
            "c",
            "c++",
            "cbl",
            "cc",
            "class",
            "clj",
            "cob",
            "cpp",
            "cs",
            "csh",
            "cxx",
            "d",
            "diff",
            "e",
            "el",
            "f",
            "f77",
            "f90",
            "fish",
            "for",
            "fth",
            "ftn",
            "go",
            "groovy",
            "h",
            "hh",
            "hpp",
            "hs",
            "html",
            "htm",
            "hxx",
            "java",
            "js",
            "jsx",
            "jsp",
            "ksh",
            "kt",
            "lhs",
            "lisp",
            "lua",
            "m",
            "m4",
            "nim",
            "patch",
            "php",
            "pl",
            "po",
            "pp",
            "py",
            "r",
            "rb",
            "rs",
            "s",
            "scala",
            "sh",
            "swg",
            "swift",
            "v",
            "vb",
            "vcxproj",
            "xcodeproj",
            "xml",
            "zsh"
        };
        static List<string> exec = new List<string>()
        {
            "exe",
            "msi",
            "bin",
            "command",
            "sh",
            "bat",
            "crx",
            "bash",
            "csh",
            "fish",
            "ksh",
            "zsh"
        };
        static List<string> font = new List<string>()
        {
            "eot",
            "otf",
            "ttf",
            "woff",
            "woff2"
        };
        static List<string> image = new List<string>()
        {
            "3dm",
            "3ds",
            "max",
            "bmp",
            "dds",
            "gif",
            "jpg",
            "jpeg",
            "png",
            "psd",
            "xcf",
            "tga",
            "thm",
            "tif",
            "tiff",
            "yuv",
            "eps",
            "svg",
            "dwg",
            "dxf",
            "gpx",
            "kml",
            "kmz",
            "webp"
        };
        static List<string> sheet = new List<string>()
        {
            "ods",
            "xls",
            "xlsx",
            "csv",
            "ics",
            "vcf"
        };
        static List<string> slide = new List<string>()
        {
            "ppt",
            "odp",
            "pptx"
        };
        static List<string> text = new List<string>()
        {
            "doc",
            "docx",
            "ebook",
            "log",
            "md",
            "msg",
            "odt",
            "org",
            "pages",
            "rtf",
            "rst",
            "tex",
            "txt",
            "wpd",
            "wps"
        };
        static List<string> video = new List<string>()
        {
            "3g2",
            "3gp",
            "aaf",
            "asf",
            "avchd",
            "avi",
            "drc",
            "flv",
            "m2v",
            "m4p",
            "m4v",
            "mkv",
            "mng",
            "mov",
            "mp2",
            "mp4",
            "mpe",
            "mpeg",
            "mpg",
            "mpv",
            "mxf",
            "nsv",
            "ogg",
            "ogv",
            "ogm",
            "qt",
            "rm",
            "rmvb",
            "roq",
            "srt",
            "svi",
            "vob",
            "webm",
            "wmv",
            "yuv"
        };

        public static string GetIcon(string extensionName)
        {
            if (extensionName != "")
                if (extensionName[0]  == '.')
                {
                    extensionName = extensionName.Remove(0, 1);
                }
            switch (extensionName)
            {
                case ".ai":
                    return "ai.png";
                case "apk":
                    return "apk.png";
                case "css":
                    return "css.png";
                case "doc":
                case "docx":
                    return "doc.png";
                case "iso":
                    return "iso.png";
                case "js":
                    return "js.png";
                case "mp3":
                    return "mp3.png";
                case "pdf":
                    return "pdf.png";
                case "ppt":
                case "pptx":
                    return "ppt.png";
                case "psd":
                    return "psd.png";
                case "svg":
                    return "svg.png";
                case "txt":
                    return "txt.png";
            }
            if (archive.Contains(extensionName))
            {
                return "zip.png";
            }
            if (audio.Contains(extensionName))
            {
                return "music.png";
            }
            if (book.Contains(extensionName))
            {
                return "text.png";
            }
            if (code.Contains(extensionName))
            {
                return "javascript.png";
            }
            if (font.Contains(extensionName))
            {
                return "font.png";
            }
            if (image.Contains(extensionName))
            {
                return "jpg.png";
            }
            if (sheet.Contains(extensionName))
            {
                return "excel.png";
            }
            if (slide.Contains(extensionName))
            {
                return "powerpoint.png";
            }
            if (text.Contains(extensionName))
            {
                return "text.png";
            }
            if (video.Contains(extensionName))
            {
                return "play.png";
            }
            return "php.png";
        }
    }
}
