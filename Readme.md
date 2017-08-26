# PSO2�ً}�\��API

## API�̐���
�����T�C�g�ً̋}�\������͂�API�Ƃ��ĕϊ�����v���O�����ł��B
���T�A���j����16:30(JST)���ɉ�͂��s���A�f�[�^���X�V���Ă��܂��B

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
[
    {
        "evant": "�ً}�N�G�X�g�P",
        "month": 8,
        "date": 22,
        "hour": 2
    },
    {
        "evant": "�ً}�N�G�X�g�Q",
        "month": 8,
        "date": 22,
        "hour": 11
    },
    {
        "evant": "�ً}�N�G�X�g�P",
        "month": 8,
        "date": 22,
        "hour": 13
    },
    {
        "evant": "�ً}�N�G�X�g�R",
        "month": 8,
        "date": 22,
        "hour": 20
    },
    {
        "evant": "�ً}�N�G�X�g�S",
        "month": 8,
        "date": 22,
        "hour": 22
    }
]
```

## �v���O�����̐���
4�̃��W���[�����琬�藧���Ă܂��B

+ POS2emaAzureFuntions
PSO2�����T�C�g��HTML����ً}���𒊏o���܂��B
���o�������́uPutDynamoDB�v���W���[���w�����܂��B
���̃v���O������AzureFunctions�œ��삵�܂��B

+ PutDynamoDB
POS2emaAzureFuntions����̃f�[�^���󂯎��DB�w�ۑ�����B
API-Gateway��AWS Lambda�Ő��藧���Ă܂��B

+ GetDynamoDB
DB��������擾���ăN���C�A���g�ɕԂ��܂��B
API-Gateway��AWS Lambda�Ő��藧���Ă܂��B

+ URL��񋟂���w
GAE�ɂē���B�{�\�[�X�R�[�h�ɂ͊܂܂�Ă��Ȃ��B

## LISENCE
MIT License
Copyright (c) 2017 aki_lua87