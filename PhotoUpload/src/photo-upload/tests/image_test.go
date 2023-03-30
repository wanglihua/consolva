package tests

import (
    "fmt"
    "image"
    "os"
    "testing"
)

func TestCorp(t *testing.T) {

    file, err := os.Open("/Users/wanglihua/Downloads/image_upload/202009/12345678902.jpg")
    if err != nil {
        panic(err)
    }
    defer file.Close()

    img, _, err := image.Decode(file) // 不知道为什么，这里会报错
    if err != nil {
        panic(err)
    }

    var color = img.At(1, 2)

    r, g, b, a := color.RGBA()

    fmt.Println(r, g, b, a)
}
