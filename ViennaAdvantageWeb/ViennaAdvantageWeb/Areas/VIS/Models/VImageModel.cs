using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Web.Helpers;
using System.Web.Hosting;
using VIS.Classes;

namespace VIS.Models
{
    public class VImageModel
    {
        public string UsrImage { get; set; }

        public VImageModel GetImage(Ctx ctx, int ad_image_id, int size)
        {
            VImageModel obj = new VImageModel();
            MImage mimg = new MImage(ctx, ad_image_id, null);
            var value = mimg.GetThumbnailByte(size, size);
            if (value != null)
            {
                obj.UsrImage = Convert.ToBase64String(value);
                //obj.UsrImage = Convert.ToBase64String(mimg.GetBinaryData());
            }
            return obj;
        }

        /// <summary>
        /// Save images
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="serverPath"></param>
        /// <param name="file"></param>
        /// <param name="imageID"></param>
        /// <param name="isDatabaseSave"></param>
        /// <returns></returns>
        public int SaveImage(Ctx ctx, string serverPath, HttpPostedFileBase file, int imageID, bool isDatabaseSave)
        {
            HttpPostedFileBase hpf = file as HttpPostedFileBase;

            string savedFileName = Path.Combine(serverPath, Path.GetFileName(hpf.FileName));
            hpf.SaveAs(savedFileName);
            MemoryStream ms = new MemoryStream();
            hpf.InputStream.CopyTo(ms);
            byte[] byteArray = ms.ToArray();
            FileInfo file1 = new FileInfo(savedFileName);
            if (file1.Exists)
            {
                file1.Delete(); //Delete Temporary file             
            }

            string imgByte = Convert.ToBase64String(byteArray);
            var id = CommonFunctions.SaveImage(ctx, byteArray, imageID, hpf.FileName.Substring(hpf.FileName.LastIndexOf('.')), isDatabaseSave);
            return id;
        }


        public object GetArrayFromFile(string serverPath, HttpPostedFileBase file)
        {
            HttpPostedFileBase hpf = file as HttpPostedFileBase;
            string savedFileName = Path.Combine(serverPath, Path.GetFileName(hpf.FileName));
            hpf.SaveAs(savedFileName);
            MemoryStream ms = new MemoryStream();
            hpf.InputStream.CopyTo(ms);
            byte[] byteArray = ms.ToArray();
            FileInfo file1 = new FileInfo(savedFileName);
            if (file1.Exists)
            {
                file1.Delete(); //Delete Temporary file             
            }
            string imgByte = Convert.ToBase64String(byteArray);
            return imgByte;
        }

    }
}