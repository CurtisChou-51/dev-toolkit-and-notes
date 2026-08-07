# C# Refact Note - UpdateGroupListingFromDB

- 對於 `UpdateGroupListingFromDB` 方法的重構筆記，調整由原始資料轉換為自訂資料結構的過程

## 重構描述

- 查找方向修正：`userPair` 原本建成 `Dictionary<uid, account>`，但所有查詢都是給 account 找 uid。改為 `ILookup<account, uid>` 
  - 效能：用 `Dictionary` 卻查 `Value` 等於沒用到雜湊，改為 `Key` 檢索才會達到 O(1)
  - 可讀性：欄位語意消失，LINQ predicate 中存取只剩 `x.Key` / `x.Value`，讓人不易理解

- 資料正規化上提：`.Trim()` 與帳號前綴原本散落在各個 predicate 中，改為在最初投影時做一次

- 移除中介方法：`initGroupListing` 的四個參數全是需要的資料，並沒有多一層抽象意義

- 其他細項：
  - 組織配對：`ForEach` + 內層 `Where` 配對改用 `GroupJoin`
  - 差集表達：原先的 `!item.UserIds.Contains(x.Key)` 依賴前面已賦值的 `item.UserIds` 且語意較不明顯，改用 `Except`
  - EF 優化：`GetAll().ToDictionary()` 會一次把整個資料表拉下來，改為 `GetAll().Select().ToLookup()` 加入投影只拉需要的欄位
  - 註解：`MEMBER_LEVEL` 的 0 與 1

## Before

```csharp
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
```

## After

```csharp
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
```

## 注意事項

- `Except` 之後可以不需要再接 `Distinct()` ：`Enumerable.Except` 本身就回傳 set，已完成去重

- 輸出順序可能改變：`UserIds` 的順序 Before 來自字典列舉，After 來自成員資料列

> [!NOTE]
> Dictionary 如果建得不對，效能沒提升還賠上可讀性  
> 這樣還不如直接對原始 `IEnumerable` 集合線性查找