# PSO2 API

[![HitCount](http://hits.dwyl.io/aki-lua87/PSO2emaAPI.svg)](http://hits.dwyl.io/aki-lua87/PSO2emaAPI)

## 説明
公式サイト(https://pso2.jp/players/boost/)を解析しAPIとして変換するプログラムです。  
毎週、水曜日の16:30(JST)頃に解析を行い、データを更新しています。  

※日本国内向けサービスのみを対象としてます
This api is target for japanese service

## API

### 詳細ドキュメント
https://aki-lua87.github.io/PSO2API/doc/index.html

### 簡易ドキュメント
```
緊急クエスト 
URL: https://pso2.akakitune87.net/api/emergency
method: post
request param: 
{
  "EventDate":"20190616",
  "EventType":"緊急"
}
response:
[
  {
    "Month": 6,
    "Date": 17,
    "Hour": 2,
    "Minute": 0,
    "EventName": "魔神城戦：不尽の狂気",
    "EventType": "緊急"
  },      
  {
    "Month": 6,
    "Date": 17,
    "Hour": 7,
    "Minute": 0,
    "EventName": "終の艦隊迎撃戦",
    "EventType": "緊急"
  }
]

紋章キャンペーン
https://pso2.akakitune87.net/api/coat_of_arms
method: get
param: none
```

## LISENCE
MIT License  
Copyright (c) 2017-2019 aki_lua87
