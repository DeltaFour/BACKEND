using Python.Runtime;

namespace DeltaFour.Application.Utils
{
    public class PythonExe
    {
        static PythonExe()
        {
            Path = Environment.GetEnvironmentVariable("FUNCTION_PYTHON_PATH")!;
        }

        private static readonly String Path;

        public double? CompareEmbeddings(List<double> embedding1, List<double> embedding2)
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Path);
                dynamic faceUtils = Py.Import("extract_embedding");
                dynamic result = faceUtils.calculate_similarity(embedding1.ToArray(), embedding2.ToArray());
                return (double?)result;
            }
        }
        
        public List<double>? ExtractEmbedding(byte[] base64Image)
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.path.append(Path);
                dynamic faceUtils = Py.Import("extract_embedding");
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
