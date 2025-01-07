# C# Refact Note - CopyVoiceFile

- 對於 `CopyVoiceFile` 方法的重構筆記

## 重構描述

- 拆分為小方法，使主流程更專注於高層處理：
  - `CopyLREncwav`: 檢查並複製未處理檔案
  - `EncwavToMonoWav`: 處理檔案的解密及格式轉換
  - `MergeTwoWavFiles`: 合併左聲道與右聲道檔案

- 資料流調整，將檔案轉換為對應的類別傳遞，而非重新讀取硬碟檔案狀態：
  - 原為 `GetFiles("*.encwav")` 之後手動拼接副檔名，取得要處理的 WAV
  - 改為使用 `EncwavToMonoWav` 與 `LINQ`，直接取得要處理的 WAV

- 引入區域變數，如 `FileHelper.Copy` 的參數，減少錯誤發生機會

## Before

```csharp
public bool CopyVoiceFile(string virtualFullPath, string destFullPath)
{
    string fileNameL = Path.GetFileNameWithoutExtension(virtualFullPath);
    string voiceTmpFolder = $"{_tmpSaveFolder}\\XXX\\{fileNameL}";

    // 複製 VoiceFileNameL.encwav
    Directory.CreateDirectory(voiceTmpFolder);
    if (!string.IsNullOrWhiteSpace(fileNameL) && File.Exists($"{_diskMountPath}\\{fileNameL}.encwav"))
        FileHelper.Copy($"{_diskMountPath}\\{fileNameL}.encwav", $"{voiceTmpFolder}\\{fileNameL}.encwav");

    // 檢查是否有 VoiceFileNameR.encwav
    string? fileNameR = _data.FirstOrDefault(x => x.VoiceFileNameL == fileNameL && !string.IsNullOrWhiteSpace(x.VoiceFileNameR))?.VoiceFileNameR;
    if (!string.IsNullOrWhiteSpace(fileNameR) && File.Exists($"{_diskMountPath}\\{fileNameR}.encwav"))
        FileHelper.Copy($"{_diskMountPath}\\{fileNameR}.encwav", $"{voiceTmpFolder}\\{fileNameR}.encwav");

    var encwavFiles = new DirectoryInfo(voiceTmpFolder).GetFiles("*.encwav");
    foreach (var f in encwavFiles)
    {
        DESCipher dESCipher = new DESCipher(_Key, _IV);;
        dESCipher.decryptFile(f.FullName, $"{voiceTmpFolder}\\temp.wav");
        VoiceTrans.ALow8KToMonoWave($"{voiceTmpFolder}\\temp.wav", $"{voiceTmpFolder}\\{Path.GetFileNameWithoutExtension(f.FullName)}.wav");
    }

    if (encwavFiles.Length == 0)
    {
        return false;
    }
    else if (encwavFiles.Length < 2)
    {
        FileHelper.Copy($"{voiceTmpFolder}\\{Path.GetFileNameWithoutExtension(encwavFiles[0].FullName)}.wav", destFullPath);
        return true;
    }
    else
    {
        using (var reader1 = new AudioFileReader($"{voiceTmpFolder}\\{Path.GetFileNameWithoutExtension(encwavFiles[0].FullName)}.wav"))
        using (var reader2 = new AudioFileReader($"{voiceTmpFolder}\\{Path.GetFileNameWithoutExtension(encwavFiles[1].FullName)}.wav"))
        {
            var mixer = new MixingSampleProvider(new[] { reader1, reader2 });
            WaveFileWriter.CreateWaveFile16(destFullPath, mixer);
        }
        return true;
    }
}
```

## After

```csharp
public bool CopyVoiceFile(string virtualFullPath, string destFullPath)
{
    string fileNameL = Path.GetFileNameWithoutExtension(virtualFullPath);
    string voiceTmpFolder = $"{_tmpSaveFolder}\\XXX\\{fileNameL}";

    var wavFiles = CopyLREncwav(fileNameL, voiceTmpFolder)
        .Select(x => EncwavToMonoWav(x, voiceTmpFolder))
        .ToList();
    return wavFiles.Count switch
    {
        0 => false,
        1 => CopySingleWavFile(wavFiles[0], destFullPath),
        _ => MergeTwoWavFiles(wavFiles[0], wavFiles[1], destFullPath)
    };
}

private List<FileInfo> CopyLREncwav(string fileNameL, string voiceTmpFolder)
{
    // 複製 VoiceFileNameL.encwav
    Directory.CreateDirectory(voiceTmpFolder);
    List<FileInfo> copiedFileInfos = new();
    copyEncwavIfExists(fileNameL);

    // 檢查是否有 VoiceFileNameR.encwav
    string? fileNameR = _data.FirstOrDefault(x => x.VoiceFileNameL == fileNameL)?.VoiceFileNameR;
    copyEncwavIfExists(fileNameR);
    return copiedFileInfos;

    void copyEncwavIfExists(string? fileName)
    {
        string src = Path.Combine(_diskMountPath, $"{fileName}.encwav");
        string dest = Path.Combine(voiceTmpFolder, $"{fileName}.encwav");
        if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(src))
            return;
        FileHelper.Copy(src, dest);
        copiedFileInfos.Add(new FileInfo(dest));
    }
}

private FileInfo EncwavToMonoWav(FileInfo encwavFi, string voiceTmpFolder)
{
    string tempWavPath = Path.Combine(voiceTmpFolder, "temp.wav");
    string outputWavPath = Path.Combine(voiceTmpFolder, $"{Path.GetFileNameWithoutExtension(encwavFi.FullName)}.wav");
    DESCipher dESCipher = new DESCipher(_Key, _IV);
    dESCipher.decryptFile(encwavFi.FullName, tempWavPath);
    VoiceTrans.ALow8KToMonoWave(tempWavPath, outputWavPath);
    return new FileInfo(outputWavPath);
}

private bool CopySingleWavFile(FileInfo wavFile, string destPath)
{
    FileHelper.Copy(wavFile.FullName, destPath);
    return true;
}

private bool MergeTwoWavFiles(FileInfo wav1, FileInfo wav2, string destPath)
{
    using var reader1 = new AudioFileReader(wav1.FullName);
    using var reader2 = new AudioFileReader(wav2.FullName);
    var mixer = new MixingSampleProvider(new[] { reader1, reader2 });
    WaveFileWriter.CreateWaveFile16(destPath, mixer);
    return true;
}
```