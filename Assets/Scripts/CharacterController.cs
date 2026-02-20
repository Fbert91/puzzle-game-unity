using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the mascot character animations and expressions
/// </summary>
public class CharacterController : MonoBehaviour
{
    [System.Serializable]
    public enum Expression
    {
        Neutral,
        Happy,
        Encouraging,
        Celebrating,
        Thinking,
        Sad
    }

    [SerializeField] private Animator animator;
    [SerializeField] private float expressionDuration = 3f;
    private Expression currentExpression = Expression.Neutral;
    private Coroutine expressionCoroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Set character expression
    /// </summary>
    public void SetExpression(Expression expression)
    {
        if (currentExpression == expression) return;

        currentExpression = expression;

        if (animator != null)
        {
            animator.SetTrigger(expression.ToString());
        }

        if (expressionCoroutine != null)
            StopCoroutine(expressionCoroutine);

        if (expression != Expression.Neutral)
        {
            expressionCoroutine = StartCoroutine(ResetExpressionAfterDuration());
        }
    }

    private IEnumerator ResetExpressionAfterDuration()
    {
        yield return new WaitForSeconds(expressionDuration);
        SetExpression(Expression.Neutral);
    }

    /// <summary>
    /// Play celebration animation
    /// </summary>
    public void PlayWinAnimation()
    {
        SetExpression(Expression.Celebrating);
        if (animator != null)
            animator.SetTrigger("Win");
    }

    /// <summary>
    /// Play hint gesture
    /// </summary>
    public void PlayHintGesture()
    {
        SetExpression(Expression.Thinking);
        if (animator != null)
            animator.SetTrigger("Hint");
    }

    /// <summary>
    /// Play encouragement gesture
    /// </summary>
    public void PlayEncouragement()
    {
        SetExpression(Expression.Encouraging);
    }

    /// <summary>
    /// Idle animation
    /// </summary>
    public void PlayIdle()
    {
        if (animator != null)
            animator.SetTrigger("Idle");
    }

    public Expression GetCurrentExpression() => currentExpression;
}
