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