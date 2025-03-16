using MasterMemory;
using MessagePack;

namespace Master
{
[MemoryTable("quest_master"), MessagePackObject]
public class Quest : IValidatable<Quest>
{
    // UniqueKeyの場合はValidate時にデフォルトで重複かの検証がされる
    [PrimaryKey] [Key(0)] public int Id { get; init; }

    [Key(1)] public string Name { get; init; }
    [Key(2)] public int RewardId { get; init; }
    [Key(3)] public int Cost { get; init; }
    [Key(4)] public MyEnum MyProperty { get; init; }

    void IValidatable<Quest>.Validate(IValidator<Quest> validator)
    {
        // 外部キー的に参照したいコレクションを取り出せる
        var items = validator.GetReferenceSet<Item>();

        // RewardIdが0以上のとき(0は報酬ナシのための特別なフラグとするため入力を許容する)
        if (RewardId > 0)
        {
            // Itemsのマスタに必ず含まれてなければ検証エラー（エラーが出ても続行はしてすべての検証結果を出す)
            items.Exists(x => x.RewardId, x => x.ItemId);
        }

        // コストは10..20でなければ検証エラー
        validator.Validate(x => x.Cost >= 10);
        validator.Validate(x => x.Cost <= 20);

        // 以下で囲った部分は一度しか呼ばれないため、データセット全体の検証をしたい時に使える
        if (validator.CallOnce())
        {
            var quests = validator.GetTableSet();
            // インデックス生成したもの以外のユニークどうかの検証(0は重複するため除いておく)
            quests.Where(x => x.RewardId != 0).Unique(x => x.RewardId);
        }
    }

    public enum MyEnum
    {
        A,
        B,
        C
    }
}

[MemoryTable("item"), MessagePackObject]
public class Item
{
    [PrimaryKey] [Key(0)] public int ItemId { get; init; }

    [Key(1)] public Content Content { get; init; }
}

public enum ContentType
{
    Apple,
}

[MessagePackObject]
public class Content
{
    [Key(0)] public ContentType Type { get; init; }
    [Key(1)] public int Id { get; init; }
    [Key(2)] public int Count { get; init; }
}
}
