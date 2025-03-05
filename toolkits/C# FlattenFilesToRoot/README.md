# C# FlattenFilesToRoot

- Flatten files to root

**before：**
```
baseFolder/
├── file1.txt
├── folder1/
│   ├── file2.txt
│   └── subfolder/
│       └── file3.txt
└── folder2/
    └── file4.txt
```

**after：**
```
baseFolder/
├── file1.txt
├── folder1_file2.txt
├── folder1_subfolder_file3.txt
└── folder2_file4.txt
```
