// // Copyright (c) 2025 xaze_
// //
// // This source code is licensed under the MIT license found in the
// // LICENSE file in the root directory of this source tree.
// //
// // I <3 🦈s :3c

using System.Collections.Generic;

namespace XazeAPI.Features.Model.Animating;

public class AnimationTransition(string sourceAnimation, string targetAnimation, double duration = 0.2)
{
    public string SourceAnimation { get; } = sourceAnimation;
    public string TargetAnimation { get; } = targetAnimation;
    public double TransitionDuration { get; } = duration;
    
    public bool HasExitTime { get; set; }
    public double ExitTime { get; set; } = 0.9;
    
    private AnimationController _controller;
    private readonly List<TransitionCondition> _conditions = new();

    public void AddCondition(string parameterName, object targetValue, ConditionMode conditionMode =  ConditionMode.Equals)
        => _conditions.Add(new TransitionCondition(parameterName, targetValue, conditionMode));
    
    public void AddCondition(TransitionCondition condition)
        => _conditions.Add(condition);
    
    internal void Setup(AnimationController controller)
    {
        _controller = controller;
        controller.ParameterChanged += OnParameterChanged;
        
        Evaluate();
    }

    internal void Remove(AnimationController controller)
    {
        controller.ParameterChanged -= OnParameterChanged;
    }

    private void OnParameterChanged(string parameterName, object value)
    {
        Evaluate();
    }

    public void Evaluate()
    {
        if (_controller.CurrentAnimationName != SourceAnimation)
            return;
        
        var sourceAnim = _controller.GetAnimation(SourceAnimation);
        if (sourceAnim == null)
            return;
        
        if (HasExitTime && sourceAnim.GetNormalizedTime() < ExitTime)
            return;

        foreach (var condition in _conditions)
        {
            object val = _controller.Get<object>(condition.ParameterName);
            if (!condition.Evaluate(val))
                return;
        }
        
        _controller.ExecuteTransition(this);
    }
}