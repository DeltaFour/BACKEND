using Python.Runtime;

namespace DeltaFour.Application.Utils
{
    public class PythonExe
    {
        static PythonExe()
        {
            Path = "FunctionPython/extract_embedding.py";
        }

        private static readonly String Path;
        public double? CompareEmbeddings(List<double> embedding1, List<double> embedding2)
        {
            using (Py.GIL())
            {
                dynamic faceUtils = Py.Import(System.IO.Path.GetFileNameWithoutExtension(Path));
                dynamic result = faceUtils.calculate_similarity(embedding1.ToArray(), embedding2.ToArray());
                return (double?)result;
            }
        }
        
        public List<double>? ExtractEmbedding(byte[] base64Image)
        {
            using (Py.GIL())
            {
                dynamic faceUtils = Py.Import(System.IO.Path.GetFileNameWithoutExtension(Path));
                dynamic result = faceUtils.extract_embedding(base64Image);
                if (result == null)
                    return null;

                var list = new List<double>();
                foreach (dynamic v in result)
                    list.Add((double)v);

                return list;
            }
        }
    }
    
    
}
