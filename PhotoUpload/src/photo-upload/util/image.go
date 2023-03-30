package util

import (
    "image"
    "math"
    "os"
    "photo-upload/config"
    "time"
)

func LoadImage(path string) (img image.Image, err error) {
    file, err := os.Open(path)
    if err != nil {
        img = nil
        return
    }
    defer file.Close()

    fileStat, err := file.Stat()

    if err != nil {
        img = nil
        return
    }

    // 10 秒钟之内的不上传
    if fileStat.ModTime().Add(time.Second * time.Duration(10)).After(time.Now()) {
        img = nil
        return
    }

    // 5 分钟之前的不上传
    if fileStat.ModTime().Add(time.Second * time.Duration(300)).Before(time.Now()) {
        img = nil
        return
    }

    img, _, err = image.Decode(file)
    return
}

func CalculateImageCutRect(img image.Image) image.Rectangle {
    var imgOriginWidth = img.Bounds().Dx()
    var imgOriginHeight = img.Bounds().Dy()

    var top = int(float64(imgOriginHeight) * 0.01)
    var right = int(float64(imgOriginWidth) * 0.3)
    var bottom = int(float64(imgOriginHeight) * 0.20)
    var left = int(float64(imgOriginWidth) * 0.18)

    return image.Rect(left, top, imgOriginWidth-right, imgOriginHeight-bottom)
}

func CalculateUploadImageSize(img image.Image) (width int, height int) {
    var imgOriginWidth = img.Bounds().Dx()
    var imgOriginHeight = img.Bounds().Dy()

    var maxOriginSize = math.Max(float64(imgOriginWidth), float64(imgOriginHeight))
    if int(maxOriginSize) <= config.UploadImageMaxSize {
        return imgOriginWidth, imgOriginHeight
    }

    var ratio = float64(config.UploadImageMaxSize) / maxOriginSize
    return int(float64(imgOriginWidth) * ratio), int(float64(imgOriginHeight) * ratio)
}
