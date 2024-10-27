using ETicaretAPI.Application.Services;
using ETicaretAPI.Infrastructure.Operations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class FileService : IFileService
    {
        readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);

                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                //todo log!
                throw ex;
            }
        }
        //projede aynı isimde dosya olup olmadığını görmek için path i de alıyoruz 
        async Task<string> FileRenameAsync(string path, string fileName, bool first = true)
        {

            string newFileName = await Task.Run<string>(async () =>
            {
                string extension = Path.GetExtension(fileName);//fileName in extension(.png gibi) alıyoruz
                string newFileName = string.Empty;// bunu düzenleyip return edeceğiz 
                if (first)// daha önce buraya girilmediye
                {
                    string oldName = Path.GetFileNameWithoutExtension(fileName);//fileName değerini extension(.png) siz alıyoruz
                    newFileName = $"{NameOperation.CharacterRegulatory(oldName)}{extension}";//dosya ismini düzenliyoruz ve sonuna da extensionını ekliyoruz
                }
                else
                {//aynı isimde dosya varsa burada dosyanın sonuna -1 gibi ifadeler koyulacak
                    newFileName = fileName;//daha önce if bloğu çalıştığından ve parametreye newfileName değeri
                                           //verildiği için aslında parametredeki fileName newFileName(yani daha önce if bloğuna girip düzeltilmiş )
                    int indexNo1 = newFileName.IndexOf("-");//IndexOf değeri bulamazise -1 dönen bir fonksiyon
                    if (indexNo1 == -1)
                        newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";//- olmadığına göre demekki budosya ilk defa tekrar ediyor 
                    else
                    {//daha önce - koyulmuş ise ozaman -2 den üst olabilir
                        int lastIndex = 0;
                        //indexNo1 içinde - olduğundan eminiz o yüzden indexNo1= (-) kaçıncı indexte ise onun değeri
                        while (true)
                        {
                            lastIndex = indexNo1;
                            indexNo1 = newFileName.IndexOf("-", indexNo1 + 1);//+1 eklememizin sebebi aramayı indexNo1 den sonraki indexten başlatmasını istiyoruz
                            if (indexNo1 == -1)                               // aksi halde zaten indexNo1 de - olduğu kesin ozaman 0 döner(yani 0.indexte var der)
                            {//eğer daha (-) yoksa
                                indexNo1 = lastIndex;//(-)daha - olmadığı için indexNo1 i son (-) bulduğu indexe yani lastIndex dönüyoruz
                                break;
                            }//hala - varsa ozaman döngüde bütün (-) leri bulana kadar tekrar ediyoruz
                        }
                        //(-) ler bitti son (-)nin indexi indexNo1 de tutuluyor (merhaba-dunya-2.jpg) indexNo1=7
                        int indexNo2 = newFileName.IndexOf(".");//(.)olduğu index= indexNo2
                        string fileNo = newFileName.Substring(indexNo1 + 1, indexNo2 - indexNo1 - 1);//no1+1 karakterden itibaren no2-no1 kadar karakteri(. ile - arasındaki) alır
                        if (int.TryParse(fileNo, out int _fileNo))//fileNo değeri int türüne dönüştürülür içinde sayı yoksa zateen else düşer varsa o sayıyı artırıp kaydetmeye çalışılacak
                        {
                            _fileNo++;//değerimizi 1 artırıyoruz yeni eklenecek dosya için
                            newFileName = newFileName.Remove(indexNo1 + 1, indexNo2 - indexNo1 - 1)//(-)den sonraki karakterden itibaren no2-no1 kadar kadar karakteri siler
                                                .Insert(indexNo1 + 1, _fileNo.ToString());//ve (-) den sonraki yere yeni _fileNo ekler
                        }
                        else
                            newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";//(-) den sonraki aralıkta sayı yoksa ve zaten buraya aynı dosyadan 
                                                                                                          //1 tane olduğu için düştü ozaman bu dosya burada 1 tane var demektir 
                    }
                }// string newfileName
                if (File.Exists($"{path}\\{newFileName}"))
                    //projede aynı isimde ve path de dosya varsa ozaman tekrar FileRenameAsync Fonkiyonunu çalıştırıyoruz ve firs=false yapıyoruz
                    return await FileRenameAsync(path, newFileName, false);
                else
                    return newFileName;
            });
            return newFileName;
        }

        public async Task<List<(string fileName, string path)>> UploadAsync(string path, IFormFileCollection files)
        {
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, path);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> datas = new();
            List<bool> results = new();
            foreach (IFormFile file in files)
            {
                string fileNewName = await FileRenameAsync(uploadPath, file.FileName);

                bool result = await CopyFileAsync($"{uploadPath}\\{fileNewName}", file);
                datas.Add((fileNewName, $"{path}\\{fileNewName}"));
                results.Add(result);
            }

            if (results.TrueForAll(r => r.Equals(true)))
                return datas;

            return null;

        }

       
    }
}