# PSO2 API

[![HitCount](http://hits.dwyl.io/aki-lua87/PSO2emaAPI.svg)](http://hits.dwyl.io/aki-lua87/PSO2emaAPI)

## 説明
公式サイトを解析しAPIとして変換するプログラムです。  
毎週、水曜日の16:30(JST)頃に解析を行い、データを更新しています。  

## API

### 詳細ドキュメント
https://aki-lua87.github.io/PSO2emaAPI/doc/index.html

### 簡易ドキュメント
```
緊急クエスト 
URL: https://pso2.akakitune87.net/api/emergency
method: post
param: 
{
  "EventDate":"20190616",
  "EventType":"緊急"
}

紋章キャンペーン
https://pso2.akakitune87.net/api/coat_of_arms
method: get
param: none
```

## LISENCE
MIT License  
Copyright (c) 2017-2019 aki_lua87