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