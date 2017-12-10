package pso2

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"time"

	"google.golang.org/appengine"
	"google.golang.org/appengine/urlfetch"
)

var PSO2LambdaURL string

type EmaListV3 struct {
	EventName string `json:"evant"`
	EventType string `json:"eventType"`
	Month     int    `json:"month"`
	Date      int    `json:"date"`
	Hour      int    `json:"hour"`
	Minute    int    `json:"minute"`
}

type EmaListV2 struct {
	EventName string `json:"evant"`
	Month     int    `json:"month"`
	Date      int    `json:"date"`
	Hour      int    `json:"hour"`
	Minute    int    `json:"minute"`
}

type EmaListV1 struct {
	EventName string `json:"evant"`
	Month     int    `json:"month"`
	Date      int    `json:"date"`
	Hour      int    `json:"hour"`
}

func GetEmaListV3(r *http.Request, eventDate, eventType string) (emaList []EmaListV3) {
	ctx := appengine.NewContext(r)

	client := &http.Client{
		Transport: &urlfetch.Transport{
			Context: ctx,
		},
		Timeout: time.Duration(15) * time.Second,
	}

	req, _ := http.NewRequest(
		"POST",
		PSO2LambdaURL,
		bytes.NewBuffer([]byte(eventDate)),
	)
	req.Header.Set("Content-Type", "application/json")

	resp, err := client.Do(req)
	if err != nil {
		return GetEmaListV3(r, eventDate, eventType) // ここは1回だけにしたい
	}
	defer resp.Body.Close()

	byteArray, _ := ioutil.ReadAll(resp.Body)

	var tempEmagList []EmaListV3
	if err := json.Unmarshal(byteArray, &tempEmagList); err != nil {
		log.Fatal(err)
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

func GetEmaListV2(r *http.Request, getKey string) []EmaListV2 {
	ctx := appengine.NewContext(r)

	client := &http.Client{
		Transport: &urlfetch.Transport{
			Context: ctx,
		},
		Timeout: time.Duration(15) * time.Second,
	}

	req, _ := http.NewRequest(
		"POST",
		PSO2LambdaURL,
		bytes.NewBuffer([]byte(getKey)),
	)
	req.Header.Set("Content-Type", "application/json")

	resp, err := client.Do(req)
	if err != nil {
		return GetEmaListV2(r, getKey) // ここは1回だけにしたい
		// return nil
	}
	defer resp.Body.Close()

	byteArray, _ := ioutil.ReadAll(resp.Body)

	var emaList []EmaListV2
	if err := json.Unmarshal(byteArray, &emaList); err != nil {
		log.Fatal(err)
	}

	return emaList
}

// リフレクションで各ヴァージョンに対応したい
func CnvEmaList(emaList []EmaListV2) []string {
	var emaListStr []string
	for _, v := range emaList {
		ema := fmt.Sprintf("%02d:%02d %s", v.Hour, v.Minute, v.EventName)
		emaListStr = append(emaListStr, ema)
	}
	return emaListStr
}

func GetEmaListV1(r *http.Request, getKey string) []EmaListV1 {
	ctx := appengine.NewContext(r)

	client := &http.Client{
		Transport: &urlfetch.Transport{
			Context: ctx,
		},
		Timeout: time.Duration(15) * time.Second,
	}

	req, _ := http.NewRequest(
		"POST",
		PSO2LambdaURL,
		bytes.NewBuffer([]byte(getKey)),
	)
	req.Header.Set("Content-Type", "application/json")

	resp, err := client.Do(req)
	if err != nil {
		return GetEmaListV1(r, getKey) // ここは1回だけにしたい
		// return nil
	}
	defer resp.Body.Close()

	byteArray, _ := ioutil.ReadAll(resp.Body)

	var emaList []EmaListV1
	if err := json.Unmarshal(byteArray, &emaList); err != nil {
		log.Fatal(err)
	}

	return emaList
}
