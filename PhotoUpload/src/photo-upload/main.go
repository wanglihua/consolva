package main

import (
    "fmt"
    "github.com/aliyun/aliyun-oss-go-sdk/oss"
    "github.com/beevik/guid"
    "github.com/disintegration/imaging"
    "github.com/robfig/cron/v3"
    "image"
    "image/jpeg"
    "io/ioutil"
    "net/url"
    "os"
    "path"
    "path/filepath"
    "photo-upload/config"
    "photo-upload/util"
    "strings"
    "time"
)

var serverApiUrl = config.ServerApiUrl
var uploadUserLoginName = config.UploadUserLoginName

var imageRootFolder = config.LocalImageFolder
var uploadedRootFolder = config.UploadedImageFolder
var smallImageFolder = filepath.Join(config.UploadedImageFolder, "small")

var sceneImageSuffix = strings.ToUpper("_Monitor")

var bucket *oss.Bucket = nil

var fileType string = "sign" // 默认是签收图片，另外的一个是 装箱图片 load

// GOOS=windows go build

func main() {

    if len(os.Args) > 1 {
        fileType = os.Args[1]
    }

    var ossEndPoint = config.OssEndPoint
    var ossAccessKeyId = config.OssAccessKeyId
    var ossAccessSecret = config.OssAccessSecret
    var ossBucketName = config.OssBucketName

    var logFolder = filepath.Join(uploadedRootFolder, "log")
    var logFile = filepath.Join(logFolder, "upload.log")

    err := os.MkdirAll(logFolder, 0777)
    if err != nil {
        fmt.Println(err)
        os.Exit(-1)
    }

    var logger = util.BuildLog(logFile)
    defer logger.Sync()

    err = os.MkdirAll(uploadedRootFolder, 0777)
    if err != nil {
        logger.Error(fmt.Sprint(err))

        os.Exit(-1)
    }

    err = os.MkdirAll(smallImageFolder, 0777)
    if err != nil {
        logger.Error(fmt.Sprint(err))
        os.Exit(-1)
    }

    var c = cron.New()

    _, err = c.AddFunc("@every 30s", func() {
        defer func() {
            if err := recover(); err != nil {
                logger.Error(fmt.Sprint(err))
            }
        }()

        // logger.Debug("job begin")

        client, err := oss.New(ossEndPoint, ossAccessKeyId, ossAccessSecret)
        if err != nil {
            panic(err)
        }

        bucket, err = client.Bucket(ossBucketName)
        if err != nil {
            panic(err)
        }

        uploadFolderImage(imageRootFolder)

        // logger.Debug("job end")
    })

    if err != nil {
        logger.Error(fmt.Sprint(err))
    }

    c.Start()

    select {}
}

func uploadFolderImage(imageFolder string) {
    files, err := ioutil.ReadDir(imageFolder)
    if err != nil {
        return
    }

    for _, file := range files {

        // 忽略子目录
        if file.IsDir() {
            // 递归调用
            uploadFolderImage(filepath.Join(imageFolder, file.Name()))
        }

        // 忽略 非jpg 和 bmp 文件
        if strings.HasSuffix(strings.ToLower(file.Name()), ".jpg") == false && strings.HasSuffix(strings.ToLower(file.Name()), ".bmp") == false {
            continue
        }

        var now = time.Now()

        var fileName = file.Name()
        var localFileFullName = filepath.Join(imageFolder, fileName)

        localimage, err := util.LoadImage(localFileFullName)
        if err != nil {
            panic(err)
        }

        if localimage == nil {
            continue
        }

        // 就是快递单号，箱号等，所以大写
        var fileNameWithoutExt = strings.ToUpper(strings.TrimSuffix(fileName, path.Ext(fileName)))

        var isScentImage = false
        if strings.HasSuffix(fileNameWithoutExt, sceneImageSuffix) {
            fileNameWithoutExt = strings.TrimSuffix(fileNameWithoutExt, sceneImageSuffix)
            isScentImage = true
        }

        var resizeImage *image.NRGBA = nil
        if isScentImage {
            var corpImage = imaging.Crop(localimage, util.CalculateImageCutRect(localimage))
            uploadImageWidth, uploadImageHeight := util.CalculateUploadImageSize(corpImage)
            resizeImage = imaging.Resize(corpImage, uploadImageWidth, uploadImageHeight, imaging.Lanczos)
        } else {
            uploadImageWidth, uploadImageHeight := util.CalculateUploadImageSize(localimage)
            resizeImage = imaging.Resize(localimage, uploadImageWidth, uploadImageHeight, imaging.Lanczos)
        }

        var uploadImage = resizeImage.SubImage(resizeImage.Rect)

        var smallImageFileName = fmt.Sprintf("%s%s", now.Format("20060102"), strings.Replace(guid.NewString(), "-", "", -1)+".jpg")
        var localSmallFileFullName = filepath.Join(smallImageFolder, smallImageFileName)

        localSmallFile, err := os.Create(localSmallFileFullName)
        if err != nil {
            panic(err)
        }

        err = jpeg.Encode(localSmallFile, uploadImage, nil)
        if err != nil {
            panic(err)
        }

        err = localSmallFile.Close()
        if err != nil {
            panic(err)
        }

        var newFileName = strings.Replace(guid.NewString(), "-", "", -1) + ".jpg"
        var objectName = fmt.Sprintf("logistics/%s/%s/%s", now.Format("200601"), now.Format("20060102"), newFileName)

        // 改了文件 github.com/aliyun/aliyun-oss-go-sdk/oss/bucket.go 第 67， 68 行
        err = bucket.PutObjectFromFile(objectName, localSmallFileFullName)
        if err != nil {
            panic(err)
        }

        // 删除生成的小尺寸文件
        err = os.Remove(localSmallFileFullName)
        if err != nil {
            panic(err)
        }

        var formData url.Values = make(url.Values)
        formData.Add("fileName", fileNameWithoutExt)
        formData.Add("photoUrl", objectName)
        formData.Add("uploaderLoginName", uploadUserLoginName)
        formData.Add("type", fileType)
        var response = util.PostForm(serverApiUrl, formData)
        if response == "success" {

            // 上传完了就删除本地图片
            err = os.Remove(localFileFullName)
            if err != nil {
                panic(err)
            }

            /*
               // var subFolder = filepath.Join(uploadedRootFolder, now.Format("200601"))
               subFolder, err := filepath.Rel(imageRootFolder, imageFolder)
               if err != nil {
                   panic(err)
               }

               var uploadedSubFolder = filepath.Join(uploadedRootFolder, subFolder)

               err = os.MkdirAll(uploadedSubFolder, 0777)
               if err != nil {
                   panic(err)
               }
               var uploadedFullFileName = filepath.Join(uploadedSubFolder, fileName)
               err = os.Rename(localFileFullName, uploadedFullFileName)
               if err != nil {
                   panic(err)
               }
            */
        } else {
            // 如果系统信息创建失败就，删除 oss 的图片
            err = bucket.DeleteObject(objectName)
            if err != nil {
                panic(err)
            }

            panic(response)
        }
    }
}
