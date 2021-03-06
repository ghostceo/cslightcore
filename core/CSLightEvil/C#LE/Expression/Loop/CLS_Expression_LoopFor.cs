﻿using System;
using System.Collections.Generic;
using System.Text;
namespace CSLE
{

    public class CLS_Expression_LoopFor : ICLS_Expression
    {
        public CLS_Expression_LoopFor(int tbegin, int tend, int lbegin, int lend)
        {
            listParam = new List<ICLS_Expression>();
            tokenBegin = tbegin;
            tokenEnd = tend;

            lineBegin = lbegin;
            lineEnd = lend;
        }
        public int lineBegin
        {
            get;
            private set;
        }
        public int lineEnd
        {
            get;
            set;
        }
        //Block的参数 一个就是一行，顺序执行，没有
        public List<ICLS_Expression> listParam
        {
            get;
            private set;
        }
        public int tokenBegin
        {
            get;
            private set;
        }
        public int tokenEnd
        {
            get;
            set;
        }
        public CLS_Content.Value ComputeValue(CLS_Content content)
        {
            content.InStack(this);
            content.DepthAdd();
            ICLS_Expression expr_init = listParam[0] as ICLS_Expression;
            if (expr_init != null) expr_init.ComputeValue(content);//expr1

            ICLS_Expression expr_continue = listParam[1] as ICLS_Expression;
            ICLS_Expression expr_step = listParam[2] as ICLS_Expression;

            ICLS_Expression expr_block = listParam[3] as ICLS_Expression;
            CLS_Content.Value vrt = null;
            for (; ; )
            {
                if (expr_continue != null && !(bool)expr_continue.ComputeValue(content).value) break;//expr2

                if (expr_block != null)
                {
                    if (expr_block is CLS_Expression_Block)
                    {
                        var v = expr_block.ComputeValue(content);
                        if (v != null)
                        {
                            if (v.breakBlock > 2) vrt = v;
                            if (v.breakBlock > 1) break;
                        }
                    }
                    else
                    {
                        content.DepthAdd();
                        bool bbreak = false;
                        var v = expr_block.ComputeValue(content);
                        if (v != null)
                        {
                            if (v.breakBlock > 2) vrt = v;
                            if (v.breakBlock > 1) bbreak = true;
                        }
                        content.DepthRemove();
                        if (bbreak) break;
                    }
                    //if (v.breakBlock == 1) continue;
                    //if (v.breakBlock == 2) break;
                    //if (v.breakBlock == 10) return v;
                }

                if (expr_step != null)
                    expr_step.ComputeValue(content);//expr3
            }
            content.DepthRemove();
            content.OutStack(this);
            return vrt;
            //for 逻辑
            //做数学计算
            //从上下文取值
            //_value = null;
        }


        public override string ToString()
        {
            return "For|";
        }
    }
}