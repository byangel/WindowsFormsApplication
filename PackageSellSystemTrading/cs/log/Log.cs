using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PackageSellSystemTrading
{
    class Log
    {
        private static FileStream mStream;
        private static StreamWriter mStreamWriter;             // 파일 쓰기를 위한 스트림

        /// <summary>
        /// Log 파일 생성
        /// </summary>
        /// <returns>StreamWriter</returns>
        private static StreamWriter GetStreamWriter()
        {
            if (mStreamWriter == null)
            {
                String  filePath = Util.GetCurrentDirectoryWithPath() + "\\logs\\log.txt";
                String  dirPath = Util.GetCurrentDirectoryWithPath() + "\\logs";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                    // 기존에 생성된 로그 파일이 있다면...
                    FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    string szCreateDate = file.LastWriteTime.ToString();
                    string szNowDate = DateTime.Now.ToString().Split(' ')[0];

                    // 금일 로그파일이 이미 생성되어 있다면...로그 내용을 추가
                    if (szCreateDate.IndexOf(szNowDate) > -1)
                    {
                        mStream = new FileStream(filePath, FileMode.Append);
                    }
                    // 금일 처음 로그파일이 생성되는거라면...신규로 파일 생성
                    else
                    {
                        mStream = new FileStream(filePath, FileMode.Create);
                    }
                }
                // 신규로 파일 생성
                else
                {
                    mStream = new FileStream(filePath, FileMode.Create);
                }

                mStreamWriter = new StreamWriter(mStream, Encoding.GetEncoding("euc-kr"));
                mStreamWriter.AutoFlush = true;
            }

            return mStreamWriter;
        }   // end function


        /// <summary>
        /// Log 파일에 메세지 기록
        /// </summary>
        /// <param name="szMsg"></param>
        public static void WriteLine(string szMsg)
        {
            GetStreamWriter().WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " :: " + szMsg);
        }   // end function


        /// <summary>
        /// Log 파일 쓰기 스트림 종료
        /// </summary>
        public static void Close()
        {
            if (mStreamWriter != null)
            {
                mStreamWriter.Close();
                mStream.Close();
            }
        }   // end function


    }   // end class
}   // end namespace
