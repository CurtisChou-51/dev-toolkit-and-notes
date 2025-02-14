# C# Refact Note - SuspiciousTradeService

- 對於 `SuspiciousTradeService.GetViewModel` 方法的重構筆記

## 重構描述

- 將相同的程式邏輯如主檔、簽核、交易資料的處理提取至獨立方法
- 提取區域變數，減少由 `pageModel` 重複向下存取的程式
- 使用 `Early Return` 減少嵌套層次
- 使用 `Pattern Matching` 語法簡化程式
  
## Before

```csharp
public class SuspiciousTradeService
{
    public CaseSuspiciousTradeModel? GetViewModel(string CaseId, int CodeId)
    {
        var pageModel = new CaseSuspiciousTradeModel()
        {
            CaseMainModel = _dataAccess.GetCaseMain(CaseId)
        };
        if (pageModel.CaseMainModel != null)
        {
            var caseTypeCode = _codeApi.GetCode("CaseType", CodeId);
            pageModel.CaseSignModel = _dataAccess.GetCaseSignModel(CaseId);

            if (pageModel.CaseSignModel == null)
                pageModel.CaseSignModel = new CaseSignModel();
            else
            {
                pageModel.SuspiciousTradeModels = _dataAccess.GetSuspiciousTrades(pageModel.CaseSignModel.TransId);
                foreach (var item in pageModel.SuspiciousTradeModels)
                {
                    if (item.FullBankAccount != null)
                    {
                        item.BankCode = item.FullBankAccount.Substring(0, 3);
                        if (string.IsNullOrWhiteSpace(item.BankCode))
                            item.BankCode = null;
                        item.BranchBankCode = item.FullBankAccount.Substring(3, 4);
                        if (string.IsNullOrWhiteSpace(item.BranchBankCode))
                            item.BranchBankCode = null;
                        item.Account = item.FullBankAccount.Substring(7);
                        if (string.IsNullOrWhiteSpace(item.Account))
                            item.Account = null;
                        item.TransType = EnumTransType.Bank;
                    }
                    else if (item.VirtualBagAddr != null)
                        item.TransType = EnumTransType.Virtual;
                    else if (item.CompanyId != null)
                        item.TransType = EnumTransType.Tax;
                }
            }

            if (pageModel.SuspiciousTradeModels == null)
                pageModel.SuspiciousTradeModels = new List<SuspiciousTradeModel>();

            pageModel.CaseMainModel.CodeTypeName = caseTypeCode.CodeTypeName;
            pageModel.CaseMainModel.CodeName = caseTypeCode.CodeName;
            pageModel.CaseMainModel.CodeId = caseTypeCode.CodeId;

            pageModel.CaseSignModel.DeptId = pageModel.CaseSignModel.DeptId ?? _currentUser.DeptId;
            pageModel.CaseSignModel.DeptName = _codeApi.GetCodeName("Dept", pageModel.CaseSignModel.DeptId);
            pageModel.CaseSignModel.Name = pageModel.CaseSignModel.Name ?? _currentUser.UserName;
            pageModel.CaseSignModel.Phone = pageModel.CaseSignModel.Phone ?? _currentUser.Tel;
            pageModel.CaseSignModel.Email = pageModel.CaseSignModel.Email ?? _currentUser.Email;
            pageModel.Files = _dataAccess.GetFiles(pageModel.CaseMainModel.FileGroup);

            if (pageModel.CaseMainModel.CaseStatus == EnumCaseStatus.TempSave && pageModel.SuspiciousTradeModels.Count == 0)
            {
                TradeTempSaveDto? dto = _tempStorage.Get<TradeTempSaveDto>("TradeTempSaveKey");
                if (dto != null)
                {
                    pageModel.SuspiciousTradeModels.Add(new SuspiciousTradeModel());
                    pageModel.SuspiciousTradeModels[0].VirtualBagAddr = dto.BagAddr;
                    pageModel.SuspiciousTradeModels[0].Account = dto.BankNo;
                    pageModel.SuspiciousTradeModels[0].CompanyId = dto.IdNo;
                    if (dto.BagAddr != null && dto.BankNo == null)
                        pageModel.SuspiciousTradeModels[0].TransType = EnumTransType.Virtual;
                    else if (dto.BagAddr == null && dto.BankNo != null)
                        pageModel.SuspiciousTradeModels[0].TransType = EnumTransType.Bank;
                }
            }
            pageModel.TradeTypes = _codeApi.GetCodes("TradeTypes");
            return pageModel;
        }
        return null;
    }
}
```

## After

```csharp
public class SuspiciousTradeService
{
    public CaseSuspiciousTradeModel? GetViewModel(string CaseId, int CodeId)
    {
        CaseMainModel? caseMain = InitCaseMain(CaseId, CodeId);
        if (caseMain == null)
            return null;
        CaseSignModel caseSign = InitCaseSign(CaseId);
        return new CaseSuspiciousTradeModel
        {
            CaseMainModel = caseMain,
            CaseSignModel = caseSign,
            SuspiciousTradeModels = InitSuspiciousTrades(caseMain.CaseStatus, caseSign.TransId),
            TradeTypes = _codeApi.GetCodes("TradeTypes"),
            Files = _dataAccess.GetFiles(caseMain.FileGroup)
        };
    }

    private CaseMainModel? InitCaseMain(string CaseId, int CodeId)
    {
        CaseMainModel? caseMain = _dataAccess.GetCaseMain(CaseId);
        if (caseMain == null)
            return null;
        var caseTypeCode = _codeApi.GetCode("CaseType", CodeId);
        caseMain.CodeTypeName = caseTypeCode.CodeTypeName;
        caseMain.CodeName = caseTypeCode.CodeName;
        caseMain.CodeId = caseTypeCode.CodeId;
        return caseMain;
    }

    private CaseSignModel InitCaseSign(string CaseId)
    {
        CaseSignModel caseSign = _dataAccess.GetCaseSignModel(CaseId) ?? new();
        caseSign.DeptId ??= _currentUser.DeptId;
        caseSign.DeptName = _codeApi.GetCodeName("Dept", caseSign.DeptId);
        caseSign.Name ??= _currentUser.UserName;
        caseSign.Phone ??= _currentUser.Tel;
        caseSign.Email ??= _currentUser.Email;
        return caseSign;
    }

    private List<SuspiciousTradeModel> InitSuspiciousTrades(EnumCaseStatus caseStatus, long transId)
    {
        List<SuspiciousTradeModel> suspiciousTrades = transId > 0 ? _dataAccess.GetSuspiciousTrades(transId) : new();
        foreach (var item in suspiciousTrades)
        {
            if (item.FullBankAccount != null)
                (item.BankCode, item.BranchBankCode, item.Account) = SplitAccount(item.FullBankAccount);
            item.TransType = item switch
            {
                { FullBankAccount: not null } => EnumTransType.Bank,
                { CompanyId: not null } => EnumTransType.Tax,
                { VirtualBagAddr: not null } => EnumTransType.Virtual,
                _ => item.TransType
            };
        }
        if (caseStatus == EnumCaseStatus.TempSave && suspiciousTrades.Count == 0 && TryGetTempSaveData() is SuspiciousTradeModel tempSaveData)
            suspiciousTrades.Add(tempSaveData);
        return suspiciousTrades;
    }

    private SuspiciousTradeModel? TryGetTempSaveData()
    {
        TradeTempSaveDto? dto = _tempStorage.Get<TradeTempSaveDto>("TradeTempSaveKey");
        return dto is null ? null : new()
        {
            VirtualBagAddr = dto.BagAddr,
            Account = dto.BankNo,
            CompanyId = dto.IdNo,
            TransType = dto switch
            {
                { BagAddr: not null, BankNo: null } => EnumTransType.Virtual,
                { BagAddr: null, BankNo: not null } => EnumTransType.Bank,
                _ => default
            }
        };
    }

    private static (string?, string?, string?) SplitAccount(string account)
    {
        return (emptyToNull(account.Substring(0, 3)), emptyToNull(account.Substring(3, 4)), emptyToNull(account.Substring(7)));
        static string? emptyToNull(string input) => string.IsNullOrWhiteSpace(input) ? null : input;
    }
}
```