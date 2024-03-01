﻿using Xnb.Reader.ContentReader;

namespace Xnb.Types.StardewValley;

[ClassReader]
public record SpecialOrderData(
    string Name,
    string Requester,
    string Duration,
    string Repeatable,
    string RequiredTags,
    string OrderType,
    string SpecialRule,
    string Text,
    string ItemToRemoveOnEnd,
    string MailToRemoveOnEnd,
    List<RandomizedElement> RandomizedElements,
    List<SpecialOrderObjectiveData> Objectives,
    List<SpecialOrderRewardData> Rewards
);
