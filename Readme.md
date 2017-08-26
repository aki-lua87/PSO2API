# PSO2緊急予告API

## APIの説明
公式サイトの緊急予告を解析しAPIとして変換するプログラムです。
毎週、水曜日の16:30(JST)頃に解析を行い、データを更新しています。

### URI

```
http://akakitune87.net/api/v1/pso2ema
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
"YYYYMMDD"
```

+ example
"20170822"

### 返却データ(JSON)
以下データの配列

| Key | 説明 |型|
|-----------|------------|-------|
| evant       |  緊急クエスト名 |文字列|
| month     |      発生月 | 数値|
| date     |      発生日 |数値 |
| hour     |      発生時 |数値 |

+ example

```
[
    {
        "evant": "緊急クエスト１",
        "month": 8,
        "date": 22,
        "hour": 2
    },
    {
        "evant": "緊急クエスト２",
        "month": 8,
        "date": 22,
        "hour": 11
    },
    {
        "evant": "緊急クエスト１",
        "month": 8,
        "date": 22,
        "hour": 13
    },
    {
        "evant": "緊急クエスト３",
        "month": 8,
        "date": 22,
        "hour": 20
    },
    {
        "evant": "緊急クエスト４",
        "month": 8,
        "date": 22,
        "hour": 22
    }
]
```

## プログラムの説明
4つのモジュールから成り立ってます。

+ POS2emaAzureFuntions
PSO2公式サイトのHTMLから緊急情報を抽出します。
抽出した情報は「PutDynamoDB」モジュールヘ送られます。
このプログラムはAzureFunctionsで動作します。

+ PutDynamoDB
POS2emaAzureFuntionsからのデータを受け取りDBヘ保存する。
API-GatewayとAWS Lambdaで成り立ってます。

+ GetDynamoDB
DBから情報を取得してクライアントに返します。
API-GatewayとAWS Lambdaで成り立ってます。

+ URLを提供する層
GAEにて動作。本ソースコードには含まれていない。

## LISENCE
MIT License
Copyright (c) 2017 aki_lua87