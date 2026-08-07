public void UpdateGroupListingFromDB()
{
    var groupNamePairs = GetGroupNamePairs();  // 取得組織代號與群組名稱
    string[] orgIds = groupNamePairs.Select(x => x.OrgId).Distinct().ToArray();

    var members = _memberRepository.GetAll()
        .Where(x => orgIds.Contains(x.ORG_ID) && x.KIND.StartsWith("main"))
        .Select(x => new { u_account = "AC" + x.MEMBER_ID.Trim(), OrgId = x.ORG_ID.Trim(), x.MEMBER_LEVEL, MEMBER_NAME = x.MEMBER_NAME.Trim() }).ToArray();

    // 帳號對應項目代號
    ILookup<string, string> accountToItems = _memberItemRepository.GetAll()
        .Where(x => orgIds.Contains(x.ORG_ID) && x.KIND.StartsWith("main"))
        .Select(x => new { u_account = "AC" + x.MEMBER_ID.Trim(), ITEM_ID = x.ITEM_ID.Trim() }).ToLookup(x => x.u_account, x => x.ITEM_ID);

    // 帳號對應 Uid
    ILookup<string, int> accountToUids = _userRepository.GetAll()
        .Select(x => new { x.uid, x.u_account }).ToLookup(x => x.u_account, x => x.uid);

    List<GroupListing> result = groupNamePairs.GroupJoin(members, g => g.OrgId, m => m.OrgId, (g, m) =>
    {
        // MEMBER_LEVEL 0 = 主管
        int[] leaderUids = m.Where(x => x.MEMBER_LEVEL == 0).SelectMany(x => accountToUids[x.u_account]).Distinct().ToArray();
        return new GroupListing
        {
            OrgId = g.OrgId,
            GroupName = g.Name,
            UserIds = leaderUids,
            Groups = m.Where(x => x.MEMBER_LEVEL == 1)  // MEMBER_LEVEL 1 = 子群組，MEMBER_NAME 即子群組名稱
                .Select(x => new SubGroup
                {
                    GroupName = x.MEMBER_NAME,
                    ItemIds = accountToItems[x.u_account].ToArray(),
                    UserIds = accountToUids[x.u_account].Except(leaderUids).ToArray()
                }).ToList()
        };
    }).ToList();

    // process result ...
}