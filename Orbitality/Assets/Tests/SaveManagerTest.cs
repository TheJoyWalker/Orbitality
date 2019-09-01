using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Storages;
using NUnit.Framework;
using Orbitality;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SaveManagerTest
    {
        [Serializable]
        struct TestSave
        {
            public int Value;
            public TestSave(int value) => Value = value;
        }

        // A Test behaves as an ordinary method
        [Test]
        public void Empty_After_Removal()
        {
            var sm = new SaveManager<TestSave>();
            sm.Save(new TestSave());
            Assert.IsNotEmpty(sm.GetSaves());

            sm.RemoveAll();
            Assert.IsEmpty(sm.GetSaves());
        }

        [Test]
        public void Save_Load()
        {
            var sm = new SaveManager<TestSave>();
            var saveName = sm.GetDefaultSaveName();
            var testSave = new TestSave(1);
            sm.Save(testSave, saveName);

            Assert.AreEqual(testSave.Value, sm.Load(saveName).Value);
        }


        [Test]
        public void Overwrite_Success()
        {
            var sm = new SaveManager<TestSave>();
            var saveName = sm.GetDefaultSaveName();
            var testSave = new TestSave(1);

            sm.Save(new TestSave(1000), saveName);
            sm.Save(testSave, saveName);
            Assert.AreEqual(testSave.Value, sm.Load(saveName).Value);
        }
    }
}
