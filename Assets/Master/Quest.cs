using MasterMemory;
using MessagePack;

namespace Master
{
    [MemoryTable("quest_master"), MessagePackObject(true)]
    public class Quest : IValidatable<Quest>
    {
        // UniqueKeyの場合はValidate時にデフォルトで重複かの検証がされる
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }
        public int RewardId { get; set; }
        public int Cost { get; set; }
        public MyEnum MyProperty { get; set; }

        void IValidatable<Quest>.Validate(IValidator<Quest> validator)
        {
            // 外部キー的に参照したいコレクションを取り出せる
            var items = validator.GetReferenceSet<Item>();

            // RewardIdが0以上のとき(0は報酬ナシのための特別なフラグとするため入力を許容する)
            if (this.RewardId > 0)
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

    [MemoryTable("item"), MessagePackObject(true)]
    public class Item
    {
        [PrimaryKey]
        public int ItemId { get; set; }
    }
}