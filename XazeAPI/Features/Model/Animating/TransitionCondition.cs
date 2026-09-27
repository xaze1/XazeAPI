// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System;

namespace XazeAPI.Features.Model.Animating;

public enum ConditionMode
{
    Equals,
    NotEquals,
    Greater,
    Less
}

public class TransitionCondition(string parameterName, object targetValue, ConditionMode mode)
{
    public string ParameterName { get; } = parameterName;
    public object TargetValue { get; } = targetValue;
    public ConditionMode Mode { get; } = mode;

    public bool Evaluate(object currentValue)
    {
        if (currentValue == null) return TargetValue == null;

        // Handle numeric comparisons using IComparable
        if (Mode is ConditionMode.Greater or ConditionMode.Less)
        {
            if (currentValue is not IComparable currentCmp || TargetValue is not IComparable targetCmp)
                return false;
            int comparison = currentCmp.CompareTo(targetCmp);
            return Mode == ConditionMode.Greater ? comparison > 0 : comparison < 0;
        }

        // Handle equality
        bool areEqual = currentValue.Equals(TargetValue);
        return Mode == ConditionMode.Equals ? areEqual : !areEqual;
    }
}