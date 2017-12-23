# PSO2緊急予告API

## APIの説明
公式サイトの緊急予告を解析しAPIとして変換するプログラムです。  
毎週、水曜日の16:30(JST)頃に解析を行い、データを更新しています。  

### URI

```
https://akakitune87.net/api/v4/pso2emergency
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

### 返却データ(JSON)
以下データの配列

| Key | 説明 |型|
|-----------|------------|-------|
| EventName       |  イベント名 |文字列|
| EventType    |  イベント種別 |文字列|
| Month     |      発生月 | 数値|
| Date     |      発生日 |数値 |
| Hour     |      発生時 |数値 |
| Minute     |      発生分 |数値 |

### イベントタイプリスト
"緊急クエスト","ライブ","カジノイベント"

### example

#### 指定なしの場合
```
{
  "EventDate":"20171224"
}
```

```
[
    {
        "EventName": "新世を成す幻創の造神",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 2,
        "Minute": 0
    },
    {
        "EventName": "壊城に舞う紅き邪竜",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 4,
        "Minute": 0
    },
    {
        "EventName": "クーナスペシャルライブ「永遠のencore Ver.X'mas」",
        "EventType": "ライブ",
        "Month": 12,
        "Date": 24,
        "Hour": 7,
        "Minute": 0
    },
    {
        "EventName": "氷上のメリークリスマス2017",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 7,
        "Minute": 30
    },
    {
        "EventName": "壊城に舞う紅き邪竜",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 9,
        "Minute": 0
    },
    {
        "EventName": "魔神城戦：不断の闘志",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 11,
        "Minute": 0
    },
    {
        "EventName": "クーナスペシャルライブ「永遠のencore Ver.X'mas」",
        "EventType": "ライブ",
        "Month": 12,
        "Date": 24,
        "Hour": 13,
        "Minute": 0
    },
    {
        "EventName": "壊城に舞う紅き邪竜",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 13,
        "Minute": 30
    },
    {
        "EventName": "氷上のメリークリスマス2017",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 15,
        "Minute": 0
    },
    {
        "EventName": "「メセタンシューター」でPSEレベル上昇確率がアップ！",
        "EventType": "カジノイベント",
        "Month": 12,
        "Date": 24,
        "Hour": 16,
        "Minute": 0
    },
    {
        "EventName": "氷上のメリークリスマス2017",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 20,
        "Minute": 0
    },
    {
        "EventName": "クーナスペシャルライブ「永遠のencore Ver.X'mas」",
        "EventType": "ライブ",
        "Month": 12,
        "Date": 24,
        "Hour": 22,
        "Minute": 0
    },
    {
        "EventName": "壊城に舞う紅き邪竜",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 22,
        "Minute": 30
    }
]
```

#### "緊急"の場合
```
{
  "EventDate":"20171224",
  "EventType":"緊急"
}
```

```
[
    {
        "EventName": "新世を成す幻創の造神",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 2,
        "Minute": 0
    },
    {
        "EventName": "壊城に舞う紅き邪竜",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 4,
        "Minute": 0
    },
    {
        "EventName": "氷上のメリークリスマス2017",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 7,
        "Minute": 30
    },
    {
        "EventName": "壊城に舞う紅き邪竜",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 9,
        "Minute": 0
    },
    {
        "EventName": "魔神城戦：不断の闘志",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 11,
        "Minute": 0
    },
    {
        "EventName": "壊城に舞う紅き邪竜",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 13,
        "Minute": 30
    },
    {
        "EventName": "氷上のメリークリスマス2017",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 15,
        "Minute": 0
    },
    {
        "EventName": "氷上のメリークリスマス2017",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 20,
        "Minute": 0
    },
    {
        "EventName": "壊城に舞う紅き邪竜",
        "EventType": "緊急",
        "Month": 12,
        "Date": 24,
        "Hour": 22,
        "Minute": 30
    }
]
```

## LISENCE
MIT License  
Copyright (c) 2017 aki_lua87