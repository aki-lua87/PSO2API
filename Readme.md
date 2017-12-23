# PSO2�ً}�\��API

## API�̐���
�����T�C�g�ً̋}�\������͂�API�Ƃ��ĕϊ�����v���O�����ł��B  
���T�A���j����16:30(JST)���ɉ�͂��s���A�f�[�^���X�V���Ă��܂��B  

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
  "EventType":"�C�x���g���"(�w�肵�Ȃ��ꍇ�͂��ׂẴC�x���g���擾)
}
```

### �ԋp�f�[�^(JSON)
�ȉ��f�[�^�̔z��

| Key | ���� |�^|
|-----------|------------|-------|
| EventName       |  �C�x���g�� |������|
| EventType    |  �C�x���g��� |������|
| Month     |      ������ | ���l|
| Date     |      ������ |���l |
| Hour     |      ������ |���l |
| Minute     |      ������ |���l |

### �C�x���g�^�C�v���X�g
"�ً}�N�G�X�g","���C�u","�J�W�m�C�x���g"

### example

#### �w��Ȃ��̏ꍇ
```
{
  "EventDate":"20171224"
}
```

```
[
    {
        "EventName": "�V���𐬂����n�̑��_",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 2,
        "Minute": 0
    },
    {
        "EventName": "���ɕ����g���ח�",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 4,
        "Minute": 0
    },
    {
        "EventName": "�N�[�i�X�y�V�������C�u�u�i����encore Ver.X'mas�v",
        "EventType": "���C�u",
        "Month": 12,
        "Date": 24,
        "Hour": 7,
        "Minute": 0
    },
    {
        "EventName": "�X��̃����[�N���X�}�X2017",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 7,
        "Minute": 30
    },
    {
        "EventName": "���ɕ����g���ח�",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 9,
        "Minute": 0
    },
    {
        "EventName": "���_���F�s�f�̓��u",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 11,
        "Minute": 0
    },
    {
        "EventName": "�N�[�i�X�y�V�������C�u�u�i����encore Ver.X'mas�v",
        "EventType": "���C�u",
        "Month": 12,
        "Date": 24,
        "Hour": 13,
        "Minute": 0
    },
    {
        "EventName": "���ɕ����g���ח�",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 13,
        "Minute": 30
    },
    {
        "EventName": "�X��̃����[�N���X�}�X2017",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 15,
        "Minute": 0
    },
    {
        "EventName": "�u���Z�^���V���[�^�[�v��PSE���x���㏸�m�����A�b�v�I",
        "EventType": "�J�W�m�C�x���g",
        "Month": 12,
        "Date": 24,
        "Hour": 16,
        "Minute": 0
    },
    {
        "EventName": "�X��̃����[�N���X�}�X2017",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 20,
        "Minute": 0
    },
    {
        "EventName": "�N�[�i�X�y�V�������C�u�u�i����encore Ver.X'mas�v",
        "EventType": "���C�u",
        "Month": 12,
        "Date": 24,
        "Hour": 22,
        "Minute": 0
    },
    {
        "EventName": "���ɕ����g���ח�",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 22,
        "Minute": 30
    }
]
```

#### "�ً}"�̏ꍇ
```
{
  "EventDate":"20171224",
  "EventType":"�ً}"
}
```

```
[
    {
        "EventName": "�V���𐬂����n�̑��_",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 2,
        "Minute": 0
    },
    {
        "EventName": "���ɕ����g���ח�",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 4,
        "Minute": 0
    },
    {
        "EventName": "�X��̃����[�N���X�}�X2017",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 7,
        "Minute": 30
    },
    {
        "EventName": "���ɕ����g���ח�",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 9,
        "Minute": 0
    },
    {
        "EventName": "���_���F�s�f�̓��u",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 11,
        "Minute": 0
    },
    {
        "EventName": "���ɕ����g���ח�",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 13,
        "Minute": 30
    },
    {
        "EventName": "�X��̃����[�N���X�}�X2017",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 15,
        "Minute": 0
    },
    {
        "EventName": "�X��̃����[�N���X�}�X2017",
        "EventType": "�ً}",
        "Month": 12,
        "Date": 24,
        "Hour": 20,
        "Minute": 0
    },
    {
        "EventName": "���ɕ����g���ח�",
        "EventType": "�ً}",
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