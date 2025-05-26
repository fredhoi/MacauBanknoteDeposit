using OpenCvSharp;
using Tensorflow;
using static Tensorflow.Binding;

namespace MacauBanknoteDeposit.Extensions
{
    public static class MatExtensions
    {
        public static Tensor ToTensor(this Mat mat)
        {
            // 獲取圖像數據
            mat.GetArray(out byte[] data);

            // 轉換為浮點數組並歸一化
            var floatArray = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
                floatArray[i] = data[i] / 255f;

            // 創建Tensor並指定形狀
            return tf.constant(
                value: floatArray,
                dtype: TF_DataType.TF_FLOAT,
                shape: new[] { 1, mat.Rows, mat.Cols, mat.Channels() } // 使用數组替代TensorShape
            );
        }
    }
}