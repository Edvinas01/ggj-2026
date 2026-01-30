using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InSun.JamOne.Core.Utilities
{
    /// <summary>
    /// Taken from and slightly modifier:
    /// https://gist.github.com/yasirkula/75ca350fb83ddcc1558d33a8ecf1483f
    /// </summary>
    public static class ScrollViewUtilities
    {
        public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint)
        {
            var contentSize = scrollView.content.rect.size;
            var viewportSize = ((RectTransform)scrollView.content.parent).rect.size;
            var contentScale = (Vector2)scrollView.content.localScale;

            contentSize.Scale(contentScale);
            focusPoint.Scale(contentScale);

            var scrollPosition = scrollView.normalizedPosition;
            if (scrollView.horizontal && contentSize.x > viewportSize.x)
            {
                scrollPosition.x = 0
                    + Mathf.Clamp01((focusPoint.x - viewportSize.x * 0.5f)
                    / (contentSize.x - viewportSize.x));
            }

            if (scrollView.vertical && contentSize.y > viewportSize.y)
            {
                scrollPosition.y = 0
                    + Mathf.Clamp01((focusPoint.y - viewportSize.y * 0.5f)
                    / (contentSize.y - viewportSize.y));
            }

            return scrollPosition;
        }

        public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform item)
        {
            var itemCenterPoint = (Vector2)scrollView.content.InverseTransformPoint(
                item.transform.TransformPoint(item.rect.center)
            );

            var contentSizeOffset = scrollView.content.rect.size;
            contentSizeOffset.Scale(scrollView.content.pivot);

            return scrollView.CalculateFocusedScrollPosition(itemCenterPoint + contentSizeOffset);
        }

        public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint)
        {
            scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
        }

        public static void FocusOnItem(this ScrollRect scrollView, RectTransform item)
        {
            scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);
        }

        public static IEnumerator FocusAtPointRoutine(
            this ScrollRect scrollView,
            Vector2 focusPoint,
            float speed
        )
        {
            var position = scrollView.CalculateFocusedScrollPosition(focusPoint);
            yield return scrollView.LerpToScrollPositionRoutine(position, speed);
        }

        public static IEnumerator FocusOnItemRoutine(
            this ScrollRect scrollView,
            RectTransform item,
            float speed
        )
        {
            var position = scrollView.CalculateFocusedScrollPosition(item);
            yield return scrollView.LerpToScrollPositionRoutine(position, speed);
        }

        public static IEnumerator LerpToScrollPositionRoutine(
            this ScrollRect scrollView,
            Vector2 targetNormalizedPos,
            float speed
        )
        {
            var initialNormalizedPos = scrollView.normalizedPosition;

            var t = 0f;
            while (t < 1f)
            {
                scrollView.normalizedPosition = Vector2.LerpUnclamped(
                    initialNormalizedPos,
                    targetNormalizedPos,
                    1f - (1f - t) * (1f - t)
                );

                yield return null;
                t += speed * Time.unscaledDeltaTime;
            }

            scrollView.normalizedPosition = targetNormalizedPos;
        }

        public static async UniTask FocusOnItemAsync(
            this ScrollRect scrollView,
            RectTransform item,
            float speed,
            CancellationToken cancellationToken = default
        )
        {
            var position = scrollView.CalculateFocusedScrollPosition(item);
            await scrollView.LerpToScrollPositionAsync(position, speed, cancellationToken: cancellationToken);
        }

        public static async UniTask LerpToScrollPositionAsync(
            this ScrollRect scrollView,
            Vector2 targetNormalizedPos,
            float speed,
            CancellationToken cancellationToken = default
        )
        {
            var initialNormalizedPos = scrollView.normalizedPosition;

            var t = 0f;
            while (t < 1f)
            {
                scrollView.normalizedPosition = Vector2.LerpUnclamped(
                    initialNormalizedPos,
                    targetNormalizedPos,
                    1f - (1f - t) * (1f - t)
                );

                await UniTask.WaitForEndOfFrame(cancellationToken);
                t += speed * Time.unscaledDeltaTime;
            }

            scrollView.normalizedPosition = targetNormalizedPos;
        }
    }
}
