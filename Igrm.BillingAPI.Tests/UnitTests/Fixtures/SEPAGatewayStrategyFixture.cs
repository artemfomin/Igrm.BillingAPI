using Igrm.BillingAPI.Strategies;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Igrm.BillingAPI.Tests.UnitTests.Fixtures
{
    public class SEPAGatewayStrategyFixture : FixtureBase
    {
        public SEPAGatewayStrategy SEPAGatewayStrategy { get; set; }

        public SEPAGatewayStrategyFixture()
        {
            var _SEPAGatewayStrategyMock = new Mock<SEPAGatewayStrategy>(BillingConfigurationService, FactoryLogger, OrderRepository, MemoryCache);

            _SEPAGatewayStrategyMock.Protected().Setup<string>("EnsureSourceDirectory").Returns("./test");
            _SEPAGatewayStrategyMock.Protected().Setup<List<string>>("GetFileList", "./test").Returns(new List<string>() { "./test" });
            _SEPAGatewayStrategyMock.Protected().Setup<XDocument>("LoadXDocument", "./test").Returns(XDocument.Parse(@"<Document xmlns=""urn:iso:std:iso:20022:tech:xsd:camt.053.001.02"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-Instance"">
<BkToCstmrStmt>
      <GrpHdr>
         <MsgId>CAMT17324320151019001</MsgId>
         <CreDtTm>2015 - 10 - 20T17: 47:01</CreDtTm>
      </GrpHdr>
      <Stmt>
         <Id>55667788992015102000001</Id>
         <CreDtTm>2015 - 10 - 20T17: 47:01</CreDtTm>
         <Acct>
            <Id>
               <Othr>
                  <Id>401234567</Id>
                  <SchmeNm>
                     <Cd>BBAN</Cd>
                  </SchmeNm>
               </Othr>
            </Id>
            <Ccy>SEK</Ccy>
            <Ownr>
               <Id>
                  <OrgId>
                     <Othr>
                        <Id>5566778899</Id>
                        <SchmeNm>
                           <Cd>BANK</Cd>
                        </SchmeNm>
                     </Othr>
                  </OrgId>
               </Id>
            </Ownr>
            <Svcr>
               <FinInstnId>
                  <BIC>HANDSESS</BIC>
                  <ClrSysMmbId>
                     <ClrSysId>
                        <Cd>SESBA</Cd>
                     </ClrSysId>
                     <MmbId>6290</MmbId>
                  </ClrSysMmbId>
               </FinInstnId>
            </Svcr>
         </Acct>
         <Bal>
            <Tp>
               <CdOrPrtry>
                  <Cd>OPBD</Cd>
               </CdOrPrtry>
            </Tp>
            <Amt Ccy=""SEK"">1900</Amt>
            <CdtDbtInd>CRDT</CdtDbtInd>
            <Dt>
               <Dt>2015 - 10 - 19</Dt>
            </Dt>
         </Bal>
         <Bal>
            <Tp>
               <CdOrPrtry>
                  <Cd>CLBD</Cd>
               </CdOrPrtry>
            </Tp>
            <Amt Ccy=""SEK"">1929</Amt>
            <CdtDbtInd>CRDT</CdtDbtInd>
            <Dt>
               <Dt>2015 - 10 - 19</Dt>
            </Dt>
         </Bal>
         <Bal>
            <Tp>
               <CdOrPrtry>
                  <Cd>CLAV</Cd>
               </CdOrPrtry>
            </Tp>
            <Amt Ccy=""SEK"">1929</Amt>
            <CdtDbtInd>CRDT</CdtDbtInd>
            <Dt>
               <Dt>2015 - 10 - 19</Dt>
            </Dt>
         </Bal>
         <TxsSummry>
            <TtlCdtNtries>
               <NbOfNtries>3</NbOfNtries>
               <Sum>44</Sum>
            </TtlCdtNtries>
            <TtlDbtNtries>
               <NbOfNtries>1</NbOfNtries>
               <Sum>15</Sum>
            </TtlDbtNtries>
         </TxsSummry>
         <Ntry>
            <NtryRef>5566778899201510200000100004</NtryRef>
            <Amt Ccy=""SEK"">15</Amt>
            <CdtDbtInd>DBIT</CdtDbtInd>
            <Sts>BOOK</Sts>
            <BookgDt>
               <Dt>2015 - 10 - 19</Dt>
            </BookgDt>
            <ValDt>
               <Dt>2015 - 10 - 19</Dt>
            </ValDt>
            <AcctSvcrRef>4669873074677905 ORDER-123</AcctSvcrRef>
            <BkTxCd>
               <Domn>
                  <Cd>PMNT</Cd>
                  <Fmly>
                     <Cd>ICDT</Cd>
                     <SubFmlyCd>ARET</SubFmlyCd>
                  </Fmly>
               </Domn>
               <Prtry>
                  <Cd>MOB</Cd>
               </Prtry>
            </BkTxCd>
            <NtryDtls>
               <TxDtls>
                  <Refs>
                     <ClrSysRef>4669873074677905</ClrSysRef>
                     <Prtry>
                        <Tp>OTHR</Tp>
                        <Ref>6290 SB - E43</Ref>
                     </Prtry>
                  </Refs>
                  <AmtDtls>
                     <InstdAmt>
                        <Amt Ccy=""SEK"">15</Amt>
                     </InstdAmt>
                     <TxAmt>
                        <Amt Ccy=""SEK"">15</Amt>
                     </TxAmt>
                  </AmtDtls>
                  <RltdPties>
                     <DbtrAcct>
                        <Id>
                           <Othr>
                              <Id>1233634284</Id>
                              <SchmeNm>
                                 <Prtry>MOBNB</Prtry>
                              </SchmeNm>
                           </Othr>
                        </Id>
                     </DbtrAcct>
                     <Cdtr>
                        <Nm>SVEN SVENSSON</Nm>
                     </Cdtr>
                     <CdtrAcct>
                        <Id>
                           <IBAN>TESTIBAN</IBAN>
                           <Othr>
                              <Id>+46769374866</Id>
                              <SchmeNm>
                                 <Prtry>MOBNB</Prtry>
                              </SchmeNm>
                           </Othr>
                        </Id>
                     </CdtrAcct>
                  </RltdPties>
                  <RltdAgts>
                     <DbtrAgt>
                        <FinInstnId>
                           <BIC>HANDSESS</BIC>
                        </FinInstnId>
                     </DbtrAgt>
                  </RltdAgts>
                  <AddtlTxInf>2015 - 10 - 19 - 12.53.34.057101</AddtlTxInf>
               </TxDtls>
            </NtryDtls>
         </Ntry>
      </Stmt>
   </BkToCstmrStmt></Document>"));

            SEPAGatewayStrategy = _SEPAGatewayStrategyMock.Object;
        }
    }
}
