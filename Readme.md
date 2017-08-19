## PSO2緊急予告API

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