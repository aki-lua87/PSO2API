# PSO2緊急予告API

## APIの説明
公式サイトの緊急予告を解析しAPIとして変換するプログラムです。  
毎週、水曜日の16:30(JST)頃に解析を行い、データを更新しています。  

現在対応しているイベントは"緊急クエスト","ライブ","カジノイベント"です。  

### URI

```
https://akakitune87.net/api/v3/pso2ema
```

### Method

```
POST
```

### Header

```
Content-Type: application/json
```

### POST Raw payload(Json)
```
{
  "EvantDate":"YYYYMMDD",
  "EventType":"イベント種別"(指定しない場合はすべてのイベントを取得)
}
```

+ example
{
  "EvantDate":"20171201",
  "EventType":"緊急"
}

### 返却データ(JSON)
以下データの配列

| Key | 説明 |型|
|-----------|------------|-------|
| evant       |  イベント名 |文字列|
| evantType    |  イベント種別 |文字列|
| month     |      発生月 | 数値|
| date     |      発生日 |数値 |
| hour     |      発生時 |数値 |
| minute     |      発生分 |数値 |

+ example

指定なしの場合
```
[
    {
        "evant": "緊急クエスト１",
		"eventType": "緊急",
        "month": 8,
        "date": 22,
        "hour": 2,
		"minute": 0
    },
	{
        "evant": "ライブ",
		"eventType": "ライブ",
        "month": 8,
        "date": 22,
        "hour": 11,
		"minute": 30
    },
    {
        "evant": "緊急クエスト２",
		"eventType": "緊急",
        "month": 8,
        "date": 22,
        "hour": 11,
		"minute": 0
    },
    {
        "evant": "カジノイベント",
		"eventType": "カジノイベント",
        "month": 8,
        "date": 22,
        "hour": 22,
		"minute": 0
    },
    {
        "evant": "緊急クエスト２",
        "month": 8,
        "date": 22,
        "hour": 22,
		"minute": 0
    }
]
```

EventType = "緊急"
```
[
    {
        "evant": "緊急クエスト１",
		"eventType": "緊急",
        "month": 8,
        "date": 22,
        "hour": 2,
		"minute": 0
    },
    {
        "evant": "緊急クエスト２",
		"eventType": "緊急",
        "month": 8,
        "date": 22,
        "hour": 11,
		"minute": 0
    },
    {
        "evant": "緊急クエスト２",
        "month": 8,
        "date": 22,
        "hour": 22,
		"minute": 0
    }
]
```

## LISENCE
MIT License  
Copyright (c) 2017 aki_lua87