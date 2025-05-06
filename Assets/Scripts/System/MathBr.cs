using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MathBr : MonoBehaviour
{
    public static void Swap(ref int a, ref int b)
    {
        int c = a;
        a = b;
        b = c;
    }
    public static void Swap(ref float a, ref float b)
    {
        float c = a;
        a = b;
        b = c;
    }
    public static void Swap(ref double a, ref double b)
    {
        double c = a;
        a = b;
        b = c;
    }
    public static void MeioEntre(int valor1, int valor2)
    {
        int value = valor1 - valor2;
        value /= 2;
        value += valor2;
        print(value);
    }
    public static void Gravity()
    {
        int F = 8;
        int A = 0;
        int M = 1;
        int T = 0;
        int N = F;
        while (A > -1 || T > 1000)
        {
            N = F - (M * T);
            A += N;
            T++;
            print("A = " + A);
        }
    }
    public static void MMC(int a, int b)
    {
        int a2 = a, b2 = b;
        bool cond = true;

        List<int> aList = new List<int>();
        List<int> bList = new List<int>();
        int count = 0;
        while (cond == true && count < 100)
        {
            count++;
            a2 += a;
            b2 += b;

            aList.Add(a2);
            bList.Add(b2);

            int aIndex = 0, bIndex = 0;
            foreach (int _a in aList)
            {
                aIndex++;
                foreach (int _b in bList)
                {
                    bIndex++;
                    if (_a == _b)
                    {
                        print("Número múltiplo achado em A em: " + aIndex + ", o número é: " + _a);
                        print("Número múltiplo achado em B em: " + bIndex + ", o número é: " + _b);
                        cond = false;
                    }
                }
            }
        }
    }
}