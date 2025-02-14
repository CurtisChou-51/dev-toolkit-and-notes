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