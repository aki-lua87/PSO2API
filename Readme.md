## PSO2�ً}�\��API

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

### POST Raw payload(Json)
```
"YYYYMMDD"
```

+ example
"20170809"

### �ԋp�f�[�^(JSON)
�ȉ��f�[�^�̔z��

| Key | ���� |�^|
|-----------|------------|-------|
| evant       |  �ً}�N�G�X�g�� |������|
| month     |      ������ | ���l|
| date     |      ������ |���l |
| hour     |      ������ |���l |

+ example

```
[{"evant":"�ً}�N�G�X�g�P","month":8,"date":22,"hour":2},{"evant":"�ً}�N�G�X�g�Q","month":8,"date":22,"hour":11},{"evant":"�ً}�N�G�X�g�P","month":8,"date":22,"hour":13},{"evant":"�ً}�N�G�X�g�R","month":8,"date":22,"hour":20},{"evant":"�ً}�N�G�X�g�S","month":8,"date":22,"hour":22}]
```