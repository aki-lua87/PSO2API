package pso2

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"time"

	"google.golang.org/appengine"
	"google.golang.org/appengine/urlfetch"
)

var PSO2LambdaURLV4 string

type EmaListV4 struct {
	EventName string `json:"EventName"`
	EventType string `json:"EventType"`
	Month     int    `json:"Month"`
	Date      int    `json:"Date"`
	Hour      int    `json:"Hour"`
	Minute    int    `json:"Minute"`
}

func GetEmaListV4(r *http.Request, eventDate, eventType string) (emaList []EmaListV4, err error) {
	ctx := appengine.NewContext(r)

	client := &http.Client{
		Transport: &urlfetch.Transport{
			Context: ctx,
		},
		Timeout: time.Duration(15) * time.Second,
	}

	req, err := http.NewRequest(
		"GET",
		PSO2LambdaURLV4+"?pkey="+eventDate,
		nil,
	)

	resp, err := client.Do(req)
	if err != nil {
		return
	}
	defer resp.Body.Close()

	byteArray, _ := ioutil.ReadAll(resp.Body)

	var tempEmagList []EmaListV4
	if err = json.Unmarshal(byteArray, &tempEmagList); err != nil {
		return
	}

	if eventType == "" || eventType == "none" {
		emaList = tempEmagList
	} else {
		for _, emag := range tempEmagList {
			if emag.EventType == eventType {
				emaList = append(emaList, emag)
			}
		}
	}
	return
}

// リフレクションで各ヴァージョンに対応したい
func CnvEmaList(emaList []EmaListV4) []string {
	var emaListStr []string
	for _, v := range emaList {
		ema := fmt.Sprintf("%02d:%02d %s", v.Hour, v.Minute, v.EventName)
		emaListStr = append(emaListStr, ema)
	}
	return emaListStr
}