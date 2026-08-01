using Sunny.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionDemo2026
{
    public class SimpleJsonHelper
    {
        #region 获取完整路径
        /// <summary>
        /// 获取应用程序目录下的配置文件夹，并在缺失时自动创建。
        /// </summary>
        /// <param name="folderName">相对于应用程序目录的文件夹名称。</param>
        /// <returns>配置文件夹的完整路径。</returns>
        private string GetFolderPath(string folderName)
        {
            string path = Path.Combine(Application.StartupPath, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        #endregion

        #region 相机点位信息
        [Serializable]
        public class Information
        {
            public string CameraInterfaceName { get; set; }
            public string CamerName { get; set; }
            public string CameraDeviceName { get; set; }
            public string Exposure { get; set; }
        }
        #endregion

        #region 文件夹点位信息
        [Serializable]
        public class FolderInformation
        {
            //文件夹路径
            public string FolderPath { get; set; }
        }
        #endregion

        #region 图片文件路径
        [Serializable]
        public class ImageFilePath
        {
            public string ImagePath { get; set; }
        }

        #endregion

        #region 上次图片来源
        [Serializable]
        public class ImageSourceInformation
        {
            public string SourceType { get; set; }
            public string SourcePath { get; set; }
        }

        /// <summary>
        /// 将最近一次图像来源类型和路径写入 JSON 文件。
        /// </summary>
        /// <param name="folderName">配置文件夹名称。</param>
        /// <param name="name">不含扩展名的配置文件名。</param>
        /// <param name="information">需要保存的图像来源信息。</param>
        public void JsonWriteImageSource(string folderName, string name, ImageSourceInformation information)
        {
            string folderPath = GetFolderPath(folderName);
            string documentPath = Path.Combine(folderPath, name + ".json");
            string json = Json.SerializeToFile(information, documentPath, Encoding.UTF8);
            File.WriteAllText(documentPath, json, Encoding.UTF8);
        }

        /// <summary>
        /// 从 JSON 文件读取最近一次图像来源信息。
        /// </summary>
        /// <param name="folderName">配置文件夹名称。</param>
        /// <param name="name">不含扩展名的配置文件名。</param>
        /// <returns>读取到的图像来源信息，文件不存在时返回 null。</returns>
        public ImageSourceInformation JsonReadImageSource(string folderName, string name)
        {
            string folderPath = GetFolderPath(folderName);
            string documentPath = Path.Combine(folderPath, name + ".json");
            if (!File.Exists(documentPath))
                return null;

            return Json.DeserializeFromFile<ImageSourceInformation>(documentPath, Encoding.UTF8);
        }
        #endregion

        #region JsonWrite
        /// <summary>
        /// 将相机接口、设备和曝光配置写入 JSON 文件。
        /// </summary>
        /// <param name="folderName">配置文件夹名称。</param>
        /// <param name="Name">不含扩展名的配置文件名。</param>
        /// <param name="point">需要保存的相机配置信息。</param>
        public void JsonWrite(string folderName, string Name, Information point)
        {
            string folderPath = GetFolderPath(folderName);
            string DocumentPath = Path.Combine(folderPath, Name + ".json");
            string json = Json.SerializeToFile(point, DocumentPath, Encoding.UTF8);
            File.WriteAllText(DocumentPath, json);
        }
        #endregion

        #region JsonRead
        /// <summary>
        /// 从 JSON 文件读取相机接口、设备和曝光配置。
        /// </summary>
        /// <param name="folderName">配置文件夹名称。</param>
        /// <param name="Name">不含扩展名的配置文件名。</param>
        /// <returns>读取到的相机配置，文件不存在时返回 null。</returns>
        public Information JsonRead(string folderName, string Name)
        {
            string folderPath = GetFolderPath(folderName);
            string DocumentPath = Path.Combine(folderPath, Name + ".json");
            if (!File.Exists(DocumentPath))
                return null;

            Information json = Json.DeserializeFromFile<Information>(DocumentPath, Encoding.UTF8);
            return json;
        }
        #endregion

        #region JsonWriteList 写入List<Information>到JSON
        /// <summary>
        /// 将相机配置列表写入 JSON 文件。
        /// </summary>
        /// <param name="folderName">配置文件夹名称。</param>
        /// <param name="Name">不含扩展名的配置文件名。</param>
        /// <param name="dataList">需要保存的相机配置列表。</param>
        public void JsonWriteList(string folderName, string Name, List<Information> dataList)
        {
            string folderPath = GetFolderPath(folderName);
            string DocumentPath = Path.Combine(folderPath, Name + ".json");
            string json = Json.SerializeToFile(dataList, DocumentPath, Encoding.UTF8);
            File.WriteAllText(DocumentPath, json);
        }
        #endregion

        #region JsonReadList 从JSON读取List<Information>
        /// <summary>
        /// 从 JSON 文件读取相机配置列表。
        /// </summary>
        /// <param name="folderName">配置文件夹名称。</param>
        /// <param name="Name">不含扩展名的配置文件名。</param>
        /// <returns>读取到的相机配置列表，文件不存在时返回空列表。</returns>
        public List<Information> JsonReadList(string folderName, string Name)
        {
            string folderPath = GetFolderPath(folderName);
            string DocumentPath = Path.Combine(folderPath, Name + ".json");
            if (!File.Exists(DocumentPath))
                return new List<Information>();

            var json = Json.DeserializeFromFile<List<Information>>(DocumentPath, Encoding.UTF8);
            return json;
        }
        #endregion




    }
}
