using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Parsers
{
    internal class StringTemplateParser
    {
        private readonly string _template;
        private string _stringWithTemplate = string.Empty;
        private readonly static List<char> _invalidChars = new List<char>
        {
            '<','>','|','\\','/','?','*',':','"'
        };

        //public bool CanUseStringTemplate { get; private set; } = true;
        private List<TemplateModel> _templateModels = new();


        public StringTemplateParser(string template)
        {
            _template = template;
        }


        public List<string> Generate(int count)
        {
            var list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                var currentTemplate = _stringWithTemplate;
                if (_templateModels.Count > 0)
                {

                    for (int j = 0; j < _templateModels.Count; j++)
                    {
                        if (_templateModels[j].TemplatePart1.Equals("index", StringComparison.OrdinalIgnoreCase))
                        {
                            var str = (_templateModels[j].IntTemplatePart3 + i).ToString().PadLeft(_templateModels[j].IntTemplatePart2, '0');
                            currentTemplate = currentTemplate.Replace($"|{j + 1}", str);
                        }
                        else if (_templateModels[j].TemplatePart1.Equals("yyyy", StringComparison.OrdinalIgnoreCase))
                        {
                            var str = DateTime.Now.Year.ToString();
                            currentTemplate = currentTemplate.Replace($"|{j + 1}", str);
                        }
                        else if (_templateModels[j].TemplatePart1.Equals("mm", StringComparison.OrdinalIgnoreCase))
                        {
                            var str = DateTime.Now.Month.ToString().PadLeft(2, '0');
                            currentTemplate = currentTemplate.Replace($"|{j + 1}", str);
                        }
                        else if (_templateModels[j].TemplatePart1.Equals("dd", StringComparison.OrdinalIgnoreCase))
                        {
                            var str = DateTime.Now.Day.ToString().PadLeft(2, '0');
                            currentTemplate = currentTemplate.Replace($"|{j + 1}", str);
                        }

                        if (j == _templateModels.Count - 1 &&
                            _templateModels.FirstOrDefault(s => s.TemplatePart1.Equals("index", StringComparison.OrdinalIgnoreCase)) == null)
                        {
                            currentTemplate += $"{i + 1}";
                        }
                    }
                }
                else
                {
                    currentTemplate += $"{i + 1}";
                }
                list.Add(currentTemplate);
            }


            return list;
        }


        public void Parse()
        {
            _templateModels.Clear();
            _stringWithTemplate = string.Empty;
            StringBuilder sb = new StringBuilder(64);  //有模板
            StringBuilder sb2 = new StringBuilder(64); //没有模板
            var leftBraceIndexes = new List<int>();
            var rightBraceIndexes = new List<int>();
            bool startStringTemplate = false;
            int index = 0;
            if (_template.Length <= 1)
            {
                throw new Exception("模板应该大于1个字符");
            }

            for (int i = 0; i < _template.Length; i++)
            {
                char current = _template[i];
                if (i < _template.Length - 1)
                {
                    if (current == '{')
                    {
                        if (_template[i + 1] != '{')
                        {
                            startStringTemplate = true;
                            leftBraceIndexes.Add(i);
                        }
                        else
                        {
                            sb.Append(current);
                            sb2.Append(current);
                            i++;
                            continue;
                        }
                    }
                    else if (current == '}')
                    {
                        if (_template[i + 1] != '}')
                        {
                            if (startStringTemplate)
                            {
                                startStringTemplate = false;
                                rightBraceIndexes.Add(i);
                                index++;
                                sb.Append($"|{index}");
                            }
                            else
                            {
                                rightBraceIndexes.Add(i);
                            }
                        }
                        else
                        {
                            sb.Append(current);
                            sb2.Append(current);
                            i++;
                            continue;
                        }
                    }
                    else
                    {
                        if (!startStringTemplate)
                        {
                            sb.Append(current);
                            sb2.Append(current);
                        }
                    }
                }
                else
                {
                    var last = _template[i];
                    var pre = _template[i - 1];
                    if (pre != '{' && pre != '}')
                    {
                        if (last == '{')
                        {
                            leftBraceIndexes.Add(i);
                        }
                        else if (last == '}')
                        {
                            if (startStringTemplate)
                            {
                                startStringTemplate = false;
                                index++;
                                sb.Append($"|{index}");
                                rightBraceIndexes.Add(i);
                            }
                            else
                            {
                                rightBraceIndexes.Add(i);
                            }
                        }
                        else
                        {
                            sb.Append(last);
                            sb2.Append(last);
                        }
                    }
                    else
                    {
                        sb.Append(last);
                        sb2.Append(last);
                    }
                }
            }

            //System.Diagnostics.Debug.WriteLine($"sb={sb}");
            //System.Diagnostics.Debug.WriteLine($"sb2={sb2}");
            var stringWithoutTemplate = sb2.ToString();

            if (leftBraceIndexes.Count != rightBraceIndexes.Count)
            {
                throw new Exception("输入模板不合法");
            }
            foreach (var c in _invalidChars)
            {
                if (stringWithoutTemplate.Contains(c))
                {
                    throw new Exception($"不能包含下列字符：{string.Join(' ', _invalidChars)}");
                }
            }

            _stringWithTemplate = sb.ToString();
            var templateCount = _stringWithTemplate.Where(s => s == '|').Count();
            for (int i = 0; i < templateCount; i++)
            {
                var leftIndex = leftBraceIndexes[i];
                var rightIndex = rightBraceIndexes[i];
                var subString = _template[leftIndex..(rightIndex + 1)];

                if (subString.StartsWith('{') && subString.EndsWith('}'))
                {
                    var templateString = subString.TrimStart('{').TrimEnd('}');
                    var splits = templateString.Split(':');

                    var flag = TemplateModel.ValidModelName.Any((s) =>
                     {
                         return s.Equals(splits[0], StringComparison.OrdinalIgnoreCase);
                     });
                    if (!flag)
                    {
                        throw new Exception("包含不合法的模板名称");
                    }

                    var templateModel = new TemplateModel()
                    {
                        TemplateString = templateString,
                        TemplatePart1 = splits[0]
                    };
                    if (templateModel.TemplatePart1.Equals("index", StringComparison.OrdinalIgnoreCase))
                    {
                        if (splits.Length == 3)
                        {
                            templateModel.TemplatePart2 = splits[1];
                            templateModel.TemplatePart3 = splits[2];
                        }
                        else if (splits.Length == 2)
                        {
                            templateModel.TemplatePart2 = splits[1];
                            templateModel.TemplatePart3 = "1";
                        }
                        else if (splits.Length == 1)
                        {
                            templateModel.TemplatePart2 = "4";
                            templateModel.TemplatePart3 = "1";
                        }
                        templateModel.IntTemplatePart2 = Convert.ToInt32(templateModel.TemplatePart2);
                        templateModel.IntTemplatePart3 = Convert.ToInt32(templateModel.TemplatePart3);
                    }

                    _templateModels.Add(templateModel);
                }
            }


        }

    }
}
