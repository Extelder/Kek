using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KariteyDefender3000
{
    public static class SafeDebug
    {
        public static void Info(string message)
        {
            Debug.Log("Пока без ошибок " + message);
        }

        public static void Warn(string message)
        {
            Debug.LogWarning("Скоро ошибочка " + message);
        }

        public static void Error(string message)
        {
            Debug.LogError("Ошибся " + message);
        }
    }

    public static class SafeMove
    {
        public static void Up(Transform originTransform, float speed)
        {
            Move(originTransform, originTransform.up, speed);
        }

        public static void Down(Transform originTransform, float speed)
        {
            Move(originTransform, -originTransform.up, speed);
        }

        public static void Forward(Transform originTransform, float speed)
        {
            Move(originTransform, originTransform.forward, speed);
        }

        public static void Back(Transform originTransform, float speed)
        {
            Move(originTransform, -originTransform.forward, speed);
        }

        public static void Right(Transform originTransform, float speed)
        {
            Move(originTransform, originTransform.right, speed);
        }

        public static void Left(Transform originTransform, float speed)
        {
            Move(originTransform, -originTransform.right, speed);
        }

        private static void Move(Transform originTransform, Vector3 direction, float speed)
        {
            originTransform.position += direction * speed;
        }

        public static void To(Transform originTransform, Transform targetTransform, float speed)
        {
            if (targetTransform == null)
            {
                SafeDebug.Error("No Target");
                return;
            }
            originTransform.position += SafeUtils.GetDirection(originTransform, targetTransform) * speed;
        }
        
        public static void PingPong(Transform originTransform, Vector3 startPosition, Vector3 offset, Transform targetTransform, float speed, float lenght)
        {
            if (targetTransform == null)
            {
                SafeDebug.Error("No Target");
                return;
            }
            
            float pingPong = Mathf.PingPong(Time.time * speed, lenght);

            originTransform.position = startPosition + offset * pingPong;
        }
    }

    public static class SafeRotate
    {
        public static void Right(Transform originTransform, float speed)
        {
            Rotate(originTransform, originTransform.right, speed);
        }
        
        public static void Left(Transform originTransform, float speed)
        {
            Rotate(originTransform, -originTransform.right, speed);
        }
        
        public static void Up(Transform originTransform, float speed)
        {
            Rotate(originTransform, originTransform.up, speed);
        }
        
        public static void Down(Transform originTransform, float speed)
        {
            Rotate(originTransform, -originTransform.up, speed);
        }

        private static void Rotate(Transform originTransform, Vector3 targetDirection, float speed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            originTransform.rotation = Quaternion.RotateTowards(originTransform.rotation, targetRotation, speed);
        }

        public static void ByAngle(Transform originTransform, Vector3 axis, float angle)
        {
            originTransform.Rotate(axis, angle);
        }
    }

    public static class SafeUtils
    {
        public static bool DistanceThreshHoldAchieved(Transform originTransform, Transform targetTransform, float distanceThreshHold)
        {
            return (Vector3.Distance(originTransform.position, targetTransform.position) <= distanceThreshHold);
        }

        public static void LookAt(Transform originTransform, Transform targetTransform, float speed)
        {
            if (targetTransform == null)
            {
                SafeDebug.Error("No Target");
                return;
            }
            Quaternion lookRotation = Quaternion.LookRotation(GetDirection(originTransform, targetTransform));
            originTransform.rotation = Quaternion.Slerp(originTransform.rotation, lookRotation, speed);
        }

        public static Vector3 GetDirection(Transform originTransform, Transform targetTransform)
        {
            return (targetTransform.position - originTransform.position).normalized;
        }
        
        public static void Require<T>(T component, string nameDescription, bool highlight = false, bool pause = false) where T : Component
        {
            if (component != null) return;

            string message = $"Потеряна ссылка: {nameDescription}\nПроверь, назначен ли объект в инспекторе, сейчас покажу тебе объект, смотри некст дебаг в варнингах.\n";
            SafeDebug.Error(message);

#if UNITY_EDITOR
            if (highlight && Selection.activeTransform != null)
            {
                EditorGUIUtility.PingObject(Selection.activeTransform.gameObject);
            }
#endif

            if (pause)
            {
                Time.timeScale = 0f;
            }

#if UNITY_EDITOR
            if (component == null)
            {
                T found = Object.FindObjectOfType<T>();
                if (found != null)
                {
                    SafeDebug.Warn($"Найден похожий компонент: {found.name}. Можешь назначить его вручную.");
                    return;
                }
                SafeDebug.Warn($"Ничего не получилось найти,подумай чутчут.");
            }
#endif
        }
        
        public static void CheckOnEternalCycle(Action loopAction, int maxMilliseconds, string loopName)
        {
            var startTime = DateTime.Now;

            try
            {
                loopAction.Invoke();
            }
            catch (Exception exception)
            {
                SafeDebug.Error($"Ошибка в цикле {loopName}: {exception}");
            }

            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;

            if (elapsed > maxMilliseconds)
            {
                SafeDebug.Warn($"Потенциально вечный цикл: {loopName}. Время выполнения: {elapsed} ms.");
#if UNITY_EDITOR
                Debug.Break();
#endif
            }
        }

        private static int _recursionDepth = 0;
        
        public static void CheckOnEternalRecurtion(Action action, int maxDepth, int maxMilliseconds, string name)
        {
            _recursionDepth++;
            if (_recursionDepth > maxDepth)
            {
                SafeDebug.Error($"Потенциально бесконечная рекурсия ({name}). Глубина: {_recursionDepth}");
#if UNITY_EDITOR
                Debug.Break();
#endif
                _recursionDepth--;
                return;
            }

            var startTime = DateTime.Now;
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                SafeDebug.Error($"Ошибка в рекурсии {name}: {e}");
            }

            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            if (elapsed > maxMilliseconds)
            {
                SafeDebug.Warn($"Рекурсия {name} выполняется слишком долго: {elapsed} ms.");
#if UNITY_EDITOR
                Debug.Break();
#endif
            }

            _recursionDepth--;
        }

        public static void CheckPlayerOnScene()
        {
            if (ObjectOnScene(PlayerCharacter.Instance))
            {
                SafeDebug.Info($"Все окей, игрок на сцене");
                return;
            }
            SafeDebug.Error($"ИГРОКА НЕТ НА СЦЕНЕ");
        }

        public static bool ObjectOnScene<T>(T component) where T : Component
        {
            return Object.FindObjectOfType<T>(component);
        }
    }
}

