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