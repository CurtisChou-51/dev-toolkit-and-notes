public void UpdateGroupListingFromDB()
{
    var groupNamePairs = GetGroupNamePairs();  // 取得組織代號與群組名稱
    var orgIds = groupNamePairs.Select(x => x.OrgId).Distinct().ToArray();

    var memberRawData = _memberRepository.GetAll().Where(x => orgIds.Contains(x.ORG_ID) && x.KIND.StartsWith("main")).ToArray();
    var itemRawData = _memberItemRepository.GetAll().Where(x => orgIds.Contains(x.ORG_ID) && x.KIND.StartsWith("main")).ToArray();
    var userPair = _userRepository.GetAll().ToDictionary(x => x.uid, x => x.u_account);

    List<GroupListing> result = initGroupListing(memberRawData, itemRawData, userPair, groupNamePairs);

    // process result ...
}

private List<GroupListing> initGroupListing(Member[] memberRawData, MemberItem[] itemRawData, Dictionary<int, string> userPair, GroupNamePair[] groupNamePairs)
{

    List<GroupListing> result = groupNamePairs.Select(x => new GroupListing
    {
        OrgId = x.OrgId,
        GroupName = x.Name,
        Groups = new List<SubGroup>(),
    }).ToList();

    result.ForEach(item =>
    {
        var accounts = memberRawData.Where(x => (x.MEMBER_LEVEL == 0) && x.ORG_ID.Trim() == item.OrgId).Select(x => "AC" + x.MEMBER_ID.Trim()).ToArray();

        item.UserIds = userPair.Where(x => accounts.Contains(x.Value)).Select(x => x.Key).ToArray();

        var groups = memberRawData.Where(x => x.MEMBER_LEVEL == 1 && x.ORG_ID.Trim() == item.OrgId).ToArray();
        item.Groups = groups.Select(matchGroup => new SubGroup
        {
            GroupName = matchGroup.MEMBER_NAME.Trim(),
            ItemIds = itemRawData.Where(x => matchGroup.MEMBER_ID.Trim() == x.MEMBER_ID.Trim()).Select(x => x.ITEM_ID.Trim()).ToArray(),
            UserIds = userPair.Where(x => "AC" + matchGroup.MEMBER_ID.Trim() == x.Value && !item.UserIds.Contains(x.Key)).Select(x => x.Key).ToArray()
        }).ToList();
    });
    return result;
}