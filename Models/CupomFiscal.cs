﻿namespace TesteSalut.Models
{
    public class CupomFiscal
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public byte[] Dados { get; set; }

        public string ContentType { get; set; }

    }
}
