## PSO2緊急予告API

### URI

```
https://kp822ivqfe.execute-api.ap-northeast-1.amazonaws.com/pso2emaget
```
### Method

```
POST
```

### Header

```
Content-Type: application/json
```

### POST Data(Json)
```
"YYYYMMDD"
```

+ example
"20170809"

### 返却データ
以下データの配列

| Key | 説明 |型|
|-----------|------------|-------|
| evant       |  緊急クエスト名 |文字列|
| month     |      発生月 | 数値|
| date     |      発生日 |数値 |
| hour     |      発生時 |数値 |