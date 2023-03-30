package util

import (
    "io/ioutil"
    "net/http"
    "net/url"
)

func PostForm(url string, data url.Values) string {
    httpResp, err := http.PostForm(url, data)
    if err != nil {
        panic(err)
    }

    if httpResp != nil {
        defer httpResp.Body.Close()
    }

    responseBytes, err := ioutil.ReadAll(httpResp.Body)
    if err != nil {
        panic(err)
    }

    return string(responseBytes)
}
