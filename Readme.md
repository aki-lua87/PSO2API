# PSO2�ً}�\��API

## API�̐���
�����T�C�g�ً̋}�\������͂�API�Ƃ��ĕϊ�����v���O�����ł��B  
���T�A���j����16:30(JST)���ɉ�͂��s���A�f�[�^���X�V���Ă��܂��B  

���ݑΉ����Ă���C�x���g��"�ً}�N�G�X�g","���C�u","�J�W�m�C�x���g"�ł��B  

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
  "EventType":"�C�x���g���"(�w�肵�Ȃ��ꍇ�͂��ׂẴC�x���g���擾)
}
```

+ example
{
  "EvantDate":"20171201",
  "EventType":"�ً}"
}

### �ԋp�f�[�^(JSON)
�ȉ��f�[�^�̔z��

| Key | ���� |�^|
|-----------|------------|-------|
| evant       |  �C�x���g�� |������|
| evantType    |  �C�x���g��� |������|
| month     |      ������ | ���l|
| date     |      ������ |���l |
| hour     |      ������ |���l |
| minute     |      ������ |���l |

+ example

�w��Ȃ��̏ꍇ
```
[
    {
        "evant": "�ً}�N�G�X�g�P",
		"eventType": "�ً}",
        "month": 8,
        "date": 22,
        "hour": 2,
		"minute": 0
    },
	{
        "evant": "���C�u",
		"eventType": "���C�u",
        "month": 8,
        "date": 22,
        "hour": 11,
		"minute": 30
    },
    {
        "evant": "�ً}�N�G�X�g�Q",
		"eventType": "�ً}",
        "month": 8,
        "date": 22,
        "hour": 11,
		"minute": 0
    },
    {
        "evant": "�J�W�m�C�x���g",
		"eventType": "�J�W�m�C�x���g",
        "month": 8,
        "date": 22,
        "hour": 22,
		"minute": 0
    },
    {
        "evant": "�ً}�N�G�X�g�Q",
        "month": 8,
        "date": 22,
        "hour": 22,
		"minute": 0
    }
]
```

EventType = "�ً}"
```
[
    {
        "evant": "�ً}�N�G�X�g�P",
		"eventType": "�ً}",
        "month": 8,
        "date": 22,
        "hour": 2,
		"minute": 0
    },
    {
        "evant": "�ً}�N�G�X�g�Q",
		"eventType": "�ً}",
        "month": 8,
        "date": 22,
        "hour": 11,
		"minute": 0
    },
    {
        "evant": "�ً}�N�G�X�g�Q",
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