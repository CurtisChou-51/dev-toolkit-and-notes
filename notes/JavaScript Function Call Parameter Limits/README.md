# JavaScript Function Call Parameter Limits

- JavaScript �������ưѼƦ��ƶq����A�ϥήi�}�B��Ůɻݭn�`�N

- �Ҧp�ϥήi�}�B��ŧ@���ǤJ `Math.max` �ѼơA�ѼƹL�h�ɷ|�ɭP `Maximum call stack size exceeded` ���~�G
![](01.png)  

```javascript
const arr = Array(150000).fill(1);
Math.max(...arr);
```

-  `Maximum call stack size exceeded` �����O��ƽեΰ��|���j�p�W�L�F����A���F�b�j�q���j�I�s�A�ǤJ�ѼƹL�h�ɤ]�|�ɭP�o�ӿ��~

## �ק��k

- �ϥ� `reduce`

```javascript
const arr = Array(150000).fill(1);
arr.reduce((a, b) => Math.max(a, b), -Infinity);
```