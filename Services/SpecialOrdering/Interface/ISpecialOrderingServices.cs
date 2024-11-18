using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using KANBAN.Libs;
using KANBAN.Services.Automapper.Interface;
using KANBAN.Services.Import;

namespace KANBAN.Services.SpecialOrdering.Interface
{

    public interface ISpecialOrderingServices
    {
        IKBNOR210 IKBNOR210 { get; }
        IKBNOR210_1 IKBNOR210_1 { get; }
        IKBNOR210_2 IKBNOR210_2 { get; }
        IKBNOR210_3 IKBNOR210_3 { get; }
        IKBNOR293 IKBNOR293 { get; }
        IKBNOR294 IKBNOR294 { get; }
        IKBNOR295 IKBNOR295 { get; }
        IKBNMS004 IKBNMS004 { get; }
        IKBNOR220 IKBNOR220 { get; }
        IKBNOR220_1 IKBNOR220_1 { get; }
        IKBNOR220_2 IKBNOR220_2 { get; }
        IKBNOR230 IKBNOR230 { get; }
        IKBNOR240 IKBNOR240 { get; }
        IKBNOR250 IKBNOR250 { get; }
        IKBNOR280 IKBNOR280 { get; }
        IKBNOR290 IKBNOR290 { get; }
        IKBNOR297 IKBNOR297 { get; }

    }
}
