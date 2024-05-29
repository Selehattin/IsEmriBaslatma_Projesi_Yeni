using System;
using System.IO;
using System.Security.Cryptography;

namespace IsEmriBaslatma_WebUI.WebUI.Helpers
{
    public class Sifreleme
    {
        public static byte[] Byte8(string deger)
        {
            char[] arrayChar = deger.ToCharArray();
            byte[] arrayByte = new byte[arrayChar.Length];
            for (int i = 0; i < arrayByte.Length; i++)
            {
                arrayByte[i] = Convert.ToByte(arrayChar[i]);
            }
            return arrayByte;
        }
        public string Sifrele(string strGiris)
        {
            string sonuc1;
            if (strGiris == "" || strGiris == null)
            {
                throw new ArgumentNullException("veri yok.");
            }
            else
            {
                byte[] aryKey = Byte8("10533066");
                byte[] aryIV = Byte8("10533066");
                RC2CryptoServiceProvider dec = new RC2CryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, dec.CreateEncryptor(aryKey, aryIV), CryptoStreamMode.Write);
                StreamWriter writer = new StreamWriter(cs);
                writer.Write(strGiris);
                writer.Flush();
                cs.FlushFinalBlock();
                writer.Flush();
                string sonuc = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
                sonuc1 = sonuc.Replace("/", "_");
                sonuc1 = sonuc1.Replace("+", "-");

                writer.Dispose();
                cs.Dispose();
                ms.Dispose();
            }
            return sonuc1;
        }

        public string Coz(string strGiris)
        {
            string strSonuc;
            if (strGiris == "" || strGiris == null)
            {
                throw new ArgumentNullException("veri yok.");
            }
            else
            {
                byte[] aryKey = Byte8("10533066");
                byte[] aryIV = Byte8("10533066");
                RC2CryptoServiceProvider cp = new RC2CryptoServiceProvider();
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(strGiris));
                CryptoStream cs = new CryptoStream(ms, cp.CreateDecryptor(aryKey, aryIV), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cs);
                strSonuc = reader.ReadToEnd();
                reader.Dispose();
                cs.Dispose();
                ms.Dispose();
            }
            return strSonuc;
        }
        public int UrlDüzeltme(string Code)
        {
            string ID = Code.Replace("-", "+").Replace("_", "/");
            int Cozulmus = Convert.ToInt32(Coz(ID));
            return Cozulmus;
        }


        //public string DESSifrele(string strGiris)
        //{
        //    string sonuc = "";
        //    if (strGiris == "" || strGiris == null)
        //    {
        //        throw new ArgumentNullException("veri yok");
        //    }
        //    else
        //    {
        //        byte[] aryKey = Byte8("10533063"); // 8 bit 
        //        byte[] aryIV = Byte8("10533063"); // 8 bit 
        //        DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(aryKey, aryIV), CryptoStreamMode.Write);
        //        StreamWriter writer = new StreamWriter(cs);
        //        writer.Write(strGiris);
        //        writer.Flush();
        //        cs.FlushFinalBlock();
        //        writer.Flush();
        //        sonuc = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        //        writer.Dispose();
        //        cs.Dispose();
        //        ms.Dispose();
        //    }
        //    return sonuc;
        //}
        //public string DESCoz(string strGiris)
        //{
        //    string strSonuc = "";
        //    if (strGiris == "" || strGiris == null)
        //    {
        //        throw new ArgumentNullException("veri yok.");
        //    }
        //    else
        //    {
        //        byte[] aryKey = Byte8("10533063");
        //        byte[] aryIV = Byte8("10533063");
        //        DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
        //        MemoryStream ms = new MemoryStream(Convert.FromBase64String(strGiris));
        //        CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(aryKey, aryIV), CryptoStreamMode.Read);
        //        StreamReader reader = new StreamReader(cs);
        //        strSonuc = reader.ReadToEnd();
        //        reader.Dispose();
        //        cs.Dispose();
        //        ms.Dispose();
        //    }
        //    return strSonuc;
        //}



    }
}