using LogicReinc.BlendFarm.Client.ImageTypes;
using LogicReinc.BlendFarm.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogicReinc.BlendFarm.Tests {
    /// <summary>
    /// Tests for rendering directly (no manager)
    /// </summary>
    [TestClass]
    public class GUITests {
        private static readonly string RESULTS_DIRECTORY = "GUI_Test_Results";
        private static readonly string TEST_IMAGE = "test/testfile";



        [ClassInitialize]
        public static void Init(TestContext context) {
            Directory.CreateDirectory(RESULTS_DIRECTORY);
        }

        [ClassCleanup]
        public static void Cleanup() {
        }

        [TestMethod]
        public void TestImageConvert() {
            var bytes = File.ReadAllBytes(TEST_IMAGE);
            ImageConverter.Convert(bytes, "PNG");
        }
    }
}