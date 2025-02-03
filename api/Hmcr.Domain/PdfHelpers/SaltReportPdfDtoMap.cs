using System.Collections.Generic;
using System.Globalization;
using Hmcr.Model.Dtos.SaltReport;

namespace Hmcr.Domain.PdfHelpers
{
    public class SaltReportPdfMapper
    {
        public Dictionary<string, string> MapDtoToPdfData(SaltReportDto dto)
        {
            var data = new Dictionary<string, string>();

            // General Information
            data["serviceArea"] = dto.ServiceArea.ToString();
            data["contactName"] = dto.ContactName;
            data["telephone"] = dto.Telephone;
            data["email"] = dto.Email;

            // Section 1
            if (dto.Sect1 != null)
            {
                data["sect1.planDeveloped"] = NormalizeRadioButtonValue(dto.Sect1.PlanDeveloped);
                data["sect1.planReviewed"] = NormalizeRadioButtonValue(dto.Sect1.PlanReviewed);
                data["sect1.planUpdated"] = NormalizeRadioButtonValue(dto.Sect1.PlanUpdated);

                if (dto.Sect1.Training != null)
                {
                    data["sect1.training.manager"] = NormalizeRadioButtonValue(dto.Sect1.Training.Manager);
                    data["sect1.training.supervisor"] = NormalizeRadioButtonValue(dto.Sect1.Training.Supervisor);
                    data["sect1.training.operator"] = NormalizeRadioButtonValue(dto.Sect1.Training.Operator);
                    data["sect1.training.mechanical"] = NormalizeRadioButtonValue(dto.Sect1.Training.Mechanical);
                    data["sect1.training.patroller"] = NormalizeRadioButtonValue(dto.Sect1.Training.Patroller);
                }

                if (dto.Sect1.Objectives != null)
                {
                    if (dto.Sect1.Objectives.MaterialStorage != null)
                    {
                        data["sect1.objectives.materialStorage.identified"] = dto.Sect1.Objectives.MaterialStorage.Identified?.ToString();
                        data["sect1.objectives.materialStorage.achieved"] = dto.Sect1.Objectives.MaterialStorage.Achieved?.ToString();
                    }

                    if (dto.Sect1.Objectives.SaltApplication != null)
                    {
                        data["sect1.objectives.saltApplication.identified"] = dto.Sect1.Objectives.SaltApplication.Identified?.ToString();
                        data["sect1.objectives.saltApplication.achieved"] = dto.Sect1.Objectives.SaltApplication.Achieved?.ToString();
                    }
                }
            }

            // Section 2
            if (dto.Sect2 != null)
            {
                data["sect2.roadTotalLength"] = dto.Sect2.RoadTotalLength.ToString();
                data["sect2.saltTotalDays"] = dto.Sect2.SaltTotalDays.ToString();
            }

            // Section 3
            if (dto.Sect3 != null)
            {
                AddMaterialData(data, "sect3.deicer", dto.Sect3.Deicer);
                AddMaterialData(data, "sect3.treatedAbrasives", dto.Sect3.TreatedAbrasives);
                AddMaterialData(data, "sect3.prewetting", dto.Sect3.Prewetting);
                AddMaterialData(data, "sect3.pretreatment", dto.Sect3.Pretreatment);
                AddMaterialData(data, "sect3.antiicing", dto.Sect3.Antiicing);

                if (dto.Sect3.MultiChlorideA != null)
                {
                    data["sect3.multiChlorideA.litres"] = dto.Sect3.MultiChlorideA.Litres?.ToString();
                    data["sect3.multiChlorideA.naclPercentage"] = dto.Sect3.MultiChlorideA.NaclPercentage?.ToString();
                    data["sect3.multiChlorideA.mgcl2Percentage"] = dto.Sect3.MultiChlorideA.Mgcl2Percentage?.ToString();
                    data["sect3.multiChlorideA.cacl2Percentage"] = dto.Sect3.MultiChlorideA.Cacl2Percentage?.ToString();
                }

                if (dto.Sect3.MultiChlorideB != null)
                {
                    data["sect3.multiChlorideB.litres"] = dto.Sect3.MultiChlorideB.Litres?.ToString();
                    data["sect3.multiChlorideB.naclPercentage"] = dto.Sect3.MultiChlorideB.NaclPercentage?.ToString();
                    data["sect3.multiChlorideB.mgcl2Percentage"] = dto.Sect3.MultiChlorideB.Mgcl2Percentage?.ToString();
                    data["sect3.multiChlorideB.cacl2Percentage"] = dto.Sect3.MultiChlorideB.Cacl2Percentage?.ToString();
                }
            }

            // Section 4
            if (dto.Sect4 != null)
            {
                data["sect4.saltStorageSitesTotal"] = dto.Sect4.SaltStorageSitesTotal?.ToString();

                if (dto.Sect4.Practices != null)
                {
                    AddPracticeData(data, "sect4.practices.allMaterialsHandled", dto.Sect4.Practices.AllMaterialsHandled);
                    AddPracticeData(data, "sect4.practices.equipmentPreventsOverloading", dto.Sect4.Practices.EquipmentPreventsOverloading);
                    AddPracticeData(data, "sect4.practices.wastewaterSystem", dto.Sect4.Practices.WastewaterSystem);
                    AddPracticeData(data, "sect4.practices.controlDiversionExternalWaters", dto.Sect4.Practices.ControlDiversionExternalWaters);
                    AddPracticeData(data, "sect4.practices.ongoingCleanup", dto.Sect4.Practices.OngoingCleanup);
                    AddPracticeData(data, "sect4.practices.riskManagementPlan", dto.Sect4.Practices.RiskManagementPlan);
                    AddPracticeData(data, "sect4.practices.drainageCollectionSystem", dto.Sect4.Practices.DrainageCollectionSystem);
                    AddPracticeData(data, "sect4.practices.municipalSewerSystem", dto.Sect4.Practices.MunicipalSewerSystem);
                    AddPracticeData(data, "sect4.practices.removalContainment", dto.Sect4.Practices.RemovalContainment);
                    AddPracticeData(data, "sect4.practices.watercourse", dto.Sect4.Practices.Watercourse);
                    AddPracticeData(data, "sect4.practices.otherDischargePoint", dto.Sect4.Practices.OtherDischargePoint);
                }

                if (dto.Sect4?.Stockpiles != null)
                {
                    int index = 1;
                    foreach (var stockpile in dto.Sect4.Stockpiles)
                    {
                        data[$"sect4.stockpiles.sitename_{index}"] = stockpile.SiteName;
                        data[$"sect4.stockpiles.motiOwned_{index}"] = stockpile.MotiOwned?.ToString() ?? "false";

                        if (stockpile.RoadSalts != null)
                        {
                            data[$"sect4.stockpiles.roadSalts.stockpilesTotal_{index}"] = stockpile.RoadSalts.StockpilesTotal?.ToString();
                            data[$"sect4.stockpiles.roadSalts.onImpermeableSurface_{index}"] = stockpile.RoadSalts.OnImpermeableSurface?.ToString();
                            data[$"sect4.stockpiles.roadSalts.underPermanentRoof_{index}"] = stockpile.RoadSalts.UnderPermanentRoof?.ToString();
                            data[$"sect4.stockpiles.roadSalts.underTarp_{index}"] = stockpile.RoadSalts.UnderTarp?.ToString();
                        }

                        if (stockpile.TreatedAbrasives != null)
                        {
                            data[$"sect4.stockpiles.treatedAbrasives.stockpilesTotal_{index}"] = stockpile.TreatedAbrasives.StockpilesTotal?.ToString();
                            data[$"sect4.stockpiles.treatedAbrasives.onImpermeableSurface_{index}"] = stockpile.TreatedAbrasives.OnImpermeableSurface?.ToString();
                            data[$"sect4.stockpiles.treatedAbrasives.underPermanentRoof_{index}"] = stockpile.TreatedAbrasives.UnderPermanentRoof?.ToString();
                            data[$"sect4.stockpiles.treatedAbrasives.underTarp_{index}"] = stockpile.TreatedAbrasives.UnderTarp?.ToString();
                        }

                        index++;
                    }
                }
            }

            // Section 5
            if (dto.Sect5 != null)
            {
                data["sect5.numberOfVehicles"] = dto.Sect5.NumberOfVehicles.ToString();
                data["sect5.vehiclesForSaltApplication"] = dto.Sect5.VehiclesForSaltApplication.ToString();
                data["sect5.vehiclesWithConveyors"] = dto.Sect5.VehiclesWithConveyors.ToString();
                data["sect5.vehiclesWithPreWettingEquipment"] = dto.Sect5.VehiclesWithPreWettingEquipment.ToString();
                data["sect5.vehiclesForDLA"] = dto.Sect5.VehiclesForDLA.ToString();
                data["sect5.regularCalibration"] = NormalizeRadioButtonValue(dto.Sect5.RegularCalibration);
                data["sect5.regularCalibrationTotal"] = dto.Sect5.RegularCalibrationTotal?.ToString();

                if (dto.Sect5.WeatherMonitoringSources != null)
                {
                    AddWeatherMonitoringSource(data, "sect5.weatherMonitoringSources.infraredThermometer", dto.Sect5.WeatherMonitoringSources.InfraredThermometer);
                    AddWeatherMonitoringSource(data, "sect5.weatherMonitoringSources.meteorologicalService", dto.Sect5.WeatherMonitoringSources.MeteorologicalService);
                    AddWeatherMonitoringSource(data, "sect5.weatherMonitoringSources.fixedRWISStations", dto.Sect5.WeatherMonitoringSources.FixedRWISStations);
                    AddWeatherMonitoringSource(data, "sect5.weatherMonitoringSources.mobileRWISMounted", dto.Sect5.WeatherMonitoringSources.MobileRWISMounted);
                }

                if (dto.Sect5.MaintenanceDecisionSupport != null)
                {
                    AddMaintenanceDecisionSupport(data, "sect5.maintenanceDecisionSupport.avl", dto.Sect5.MaintenanceDecisionSupport.AVL);
                    AddMaintenanceDecisionSupport(data, "sect5.maintenanceDecisionSupport.saltApplicationRates", dto.Sect5.MaintenanceDecisionSupport.SaltApplicationRates);
                    AddMaintenanceDecisionSupport(data, "sect5.maintenanceDecisionSupport.applicationRateChart", dto.Sect5.MaintenanceDecisionSupport.ApplicationRateChart);
                    AddMaintenanceDecisionSupport(data, "sect5.maintenanceDecisionSupport.testingMDSS", dto.Sect5.MaintenanceDecisionSupport.TestingMDSS);
                }
            }

            // Section 6
            if (dto.Sect6 != null)
            {
                AddSnowDisposalData(data, "sect6.disposal", dto.Sect6.Disposal);
                AddSnowDisposalData(data, "sect6.snowMelter", dto.Sect6.SnowMelter);
                AddSnowDisposalData(data, "sect6.meltwater", dto.Sect6.Meltwater);
            }

            // Section 7
            if (dto.Sect7 != null)
            {
                data["sect7.completedInventory"] = NormalizeRadioButtonValue(dto.Sect7.CompletedInventory);
                data["sect7.setVulnerableAreas"] = NormalizeRadioButtonValue(dto.Sect7.SetVulnerableAreas);
                data["sect7.actionPlanPrepared"] = NormalizeRadioButtonValue(dto.Sect7.ActionPlanPrepared);
                data["sect7.protectionMeasuresImplemented"] = NormalizeRadioButtonValue(dto.Sect7.ProtectionMeasuresImplemented);
                data["sect7.environmentalMonitoringConducted"] = NormalizeRadioButtonValue(dto.Sect7.EnvironmentalMonitoringConducted);

                if (dto.Sect7.TypesOfVulnerableAreas != null)
                {
                    AddVulnerableAreaData(data, "sect7.typesOfVulnerableAreas.drinkingWater", dto.Sect7.TypesOfVulnerableAreas.DrinkingWater);
                    AddVulnerableAreaData(data, "sect7.typesOfVulnerableAreas.aquaticLife", dto.Sect7.TypesOfVulnerableAreas.AquaticLife);
                    AddVulnerableAreaData(data, "sect7.typesOfVulnerableAreas.wetlands", dto.Sect7.TypesOfVulnerableAreas.Wetlands);
                    AddVulnerableAreaData(data, "sect7.typesOfVulnerableAreas.delimitedAreas", dto.Sect7.TypesOfVulnerableAreas.DelimitedAreas);
                    AddVulnerableAreaData(data, "sect7.typesOfVulnerableAreas.valuedLands", dto.Sect7.TypesOfVulnerableAreas.ValuedLands);
                }
            }

            // Appendices
            if (dto.Appendix != null)
            {
                AddAppendixData(data, "appendix.materialStorage.newSaltDomeWithPad", dto.Appendix.MaterialStorage.NewSaltDomeWithPad);
                AddAppendixData(data, "appendix.materialStorage.newSaltDomeIndoorStorage", dto.Appendix.MaterialStorage.NewSaltDomeIndoorStorage);
                AddAppendixData(data, "appendix.materialStorage.upgradeSaltStorageSites", dto.Appendix.MaterialStorage.UpgradeSaltStorageSites);
                AddAppendixData(data, "appendix.materialStorage.constructPermanentCoverStructure", dto.Appendix.MaterialStorage.ConstructPermanentCoverStructure);
                AddAppendixData(data, "appendix.materialStorage.impermeablePadForAbrasives", dto.Appendix.MaterialStorage.ImpermeablePadForAbrasives);
                AddAppendixData(data, "appendix.materialStorage.expandInsideBuildingForAbrasives", dto.Appendix.MaterialStorage.ExpandInsideBuildingForAbrasives);
                AddAppendixData(data, "appendix.materialStorage.useTarpsForAbrasives", dto.Appendix.MaterialStorage.UseTarpsForAbrasives);
                AddAppendixData(data, "appendix.materialStorage.reconfigureStorageCapacity", dto.Appendix.MaterialStorage.ReconfigureStorageCapacity);
                AddAppendixData(data, "appendix.materialStorage.reconfigureOperationFacilities", dto.Appendix.MaterialStorage.ReconfigureOperationFacilities);
                AddAppendixData(data, "appendix.materialStorage.designAreaForTruckLoading", dto.Appendix.MaterialStorage.DesignAreaForTruckLoading);
                AddAppendixData(data, "appendix.materialStorage.controlTruckLoading", dto.Appendix.MaterialStorage.ControlTruckLoading);
                AddAppendixData(data, "appendix.materialStorage.installEquipmentWashBay", dto.Appendix.MaterialStorage.InstallEquipmentWashBay);
                AddAppendixData(data, "appendix.materialStorage.designSiteForRunoffControl", dto.Appendix.MaterialStorage.DesignSiteForRunoffControl);
                AddAppendixData(data, "appendix.materialStorage.manageSaltContaminatedWaters", dto.Appendix.MaterialStorage.ManageSaltContaminatedWaters);
                AddAppendixData(data, "appendix.materialStorage.spillPreventionPlan", dto.Appendix.MaterialStorage.SpillPreventionPlan);
                AddAppendixData(data, "appendix.materialStorage.removeContaminatedSnow", dto.Appendix.MaterialStorage.RemoveContaminatedSnow);
                AddAppendixData(data, "appendix.materialStorage.otherSpecify", dto.Appendix.MaterialStorage.OtherSpecify);
                AddAppendixData(data, "appendix.saltApplication.installGroundSpeedControls", dto.Appendix.SaltApplication.InstallGroundSpeedControls);
                AddAppendixData(data, "appendix.saltApplication.increasePreWettingEquipment", dto.Appendix.SaltApplication.IncreasePreWettingEquipment);
                AddAppendixData(data, "appendix.saltApplication.installLiquidAntiIcing", dto.Appendix.SaltApplication.InstallLiquidAntiIcing);
                AddAppendixData(data, "appendix.saltApplication.installInfraredThermometers", dto.Appendix.SaltApplication.InstallInfraredThermometers);
                AddAppendixData(data, "appendix.saltApplication.installAdditionalRWISStations", dto.Appendix.SaltApplication.InstallAdditionalRWISStations);
                AddAppendixData(data, "appendix.saltApplication.accessRWISData", dto.Appendix.SaltApplication.AccessRWISData);
                AddAppendixData(data, "appendix.saltApplication.installMobileRWIS", dto.Appendix.SaltApplication.InstallMobileRWIS);
                AddAppendixData(data, "appendix.saltApplication.accessMeteorologicalService", dto.Appendix.SaltApplication.AccessMeteorologicalService);
                AddAppendixData(data, "appendix.saltApplication.adoptPreWettingMajorityNetwork", dto.Appendix.SaltApplication.AdoptPreWettingMajorityNetwork);
                AddAppendixData(data, "appendix.saltApplication.usePreTreatedSalt", dto.Appendix.SaltApplication.UsePreTreatedSalt);
                AddAppendixData(data, "appendix.saltApplication.adoptPreWettingOrTreatmentAbrasives", dto.Appendix.SaltApplication.AdoptPreWettingOrTreatmentAbrasives);
                AddAppendixData(data, "appendix.saltApplication.testingNewProducts", dto.Appendix.SaltApplication.TestingNewProducts);
                AddAppendixData(data, "appendix.saltApplication.adoptAntiIcingStandard", dto.Appendix.SaltApplication.AdoptAntiIcingStandard);
                AddAppendixData(data, "appendix.saltApplication.installGPSAndComputerSystems", dto.Appendix.SaltApplication.InstallGPSAndComputerSystems);
                AddAppendixData(data, "appendix.saltApplication.useChartForApplicationRates", dto.Appendix.SaltApplication.UseChartForApplicationRates);
                AddAppendixData(data, "appendix.saltApplication.useMDSS", dto.Appendix.SaltApplication.UseMDSS);
                AddAppendixData(data, "appendix.saltApplication.reviewSaltUse", dto.Appendix.SaltApplication.ReviewSaltUse);
                AddAppendixData(data, "appendix.saltApplication.assessPlowingEfficiency", dto.Appendix.SaltApplication.AssessPlowingEfficiency);
                AddAppendixData(data, "appendix.saltApplication.other", dto.Appendix.SaltApplication.Other);
                AddAppendixData(data, "appendix.snowDisposal.developProgramPhaseOut", dto.Appendix.SnowDisposal.DevelopProgramPhaseOut);
                AddAppendixData(data, "appendix.snowDisposal.installNewSiteLowPermeability", dto.Appendix.SnowDisposal.InstallNewSiteLowPermeability);
                AddAppendixData(data, "appendix.snowDisposal.upgradeExistingSiteLowPermeability", dto.Appendix.SnowDisposal.UpgradeExistingSiteLowPermeability);
                AddAppendixData(data, "appendix.snowDisposal.collectMeltWaterSpecificPoint", dto.Appendix.SnowDisposal.CollectMeltWaterSpecificPoint);
                AddAppendixData(data, "appendix.snowDisposal.constructCollectionPond", dto.Appendix.SnowDisposal.ConstructCollectionPond);
                AddAppendixData(data, "appendix.snowDisposal.otherSnowDisposal", dto.Appendix.SnowDisposal.OtherSnowDisposal);
                AddAppendixData(data, "appendix.vulnerableAreas.identifySaltVulnerableAreas", dto.Appendix.VulnerableAreas.IdentifySaltVulnerableAreas);
                AddAppendixData(data, "appendix.vulnerableAreas.prioritizeAreasForAdditionalProtection", dto.Appendix.VulnerableAreas.PrioritizeAreasForAdditionalProtection);
                AddAppendixData(data, "appendix.vulnerableAreas.implementProtectionMitigationMeasures", dto.Appendix.VulnerableAreas.ImplementProtectionMitigationMeasures);
                AddAppendixData(data, "appendix.vulnerableAreas.conductEnvironmentalMonitoring", dto.Appendix.VulnerableAreas.ConductEnvironmentalMonitoring);
                AddAppendixData(data, "appendix.vulnerableAreas.otherVulnerableAreas", dto.Appendix.VulnerableAreas.OtherVulnerableAreas);
            }

            return data;
        }

        private void AddMaterialData(Dictionary<string, string> data, string prefix, Sect3Dto.MaterialDto material)
        {
            if (material != null)
            {
                data[$"{prefix}.sandStoneDust"] = material.SandstoneDust?.ToString();
                data[$"{prefix}.nacl"] = material.Nacl?.ToString();
                data[$"{prefix}.mgcl2"] = material.Mgcl2?.ToString();
                data[$"{prefix}.cacl2"] = material.Cacl2?.ToString();
                data[$"{prefix}.acetate"] = material.Acetate?.ToString();
                data[$"{prefix}.nonchloride"] = material.Nonchloride?.ToString();
            }
        }

        private void AddPracticeData(Dictionary<string, string> data, string prefix, Sect4Dto.PracticesDto.PracticeItemDto practice)
        {
            if (practice != null)
            {
                data[$"{prefix}.hasPlan"] = NormalizeRadioButtonValue(practice.HasPlan).ToString();
                data[$"{prefix}.numSites"] = practice.NumSites?.ToString();
            }
        }

        private void AddWeatherMonitoringSource(Dictionary<string, string> data, string prefix, Sect5Dto.WeatherMonitoringSourcesDto.WMSDto source)
        {
            if (source != null)
            {
                data[$"{prefix}.relied"] = NormalizeRadioButtonValue(source.Relied);
                data[$"{prefix}.number"] = source.Number?.ToString();
            }
        }

        private void AddMaintenanceDecisionSupport(Dictionary<string, string> data, string prefix, Sect5Dto.MaintenanceDecisionSupportDto.MDSDto source)
        {
            if (source != null)
            {
                data[$"{prefix}.relied"] = NormalizeRadioButtonValue(source.Relied);
                data[$"{prefix}.number"] = source.Number?.ToString();
            }
        }

        private void AddSnowDisposalData(Dictionary<string, string> data, string prefix, Sect6Dto.SnowDisposalDto disposal)
        {
            if (disposal != null)
            {
                data[$"{prefix}.used"] = NormalizeRadioButtonValue(disposal.Used);
                data[$"{prefix}.total"] = disposal.Total?.ToString();
            }
        }

        private void AddVulnerableAreaData(Dictionary<string, string> data, string prefix, Sect7Dto.TypesOfVulnerableAreasDto.AreaDto area)
        {
            if (area != null)
            {
                data[$"{prefix}.areasIdentified"] = area.AreasIdentified?.ToString();
                data[$"{prefix}.areasWithProtection"] = area.AreasWithProtection?.ToString();
                data[$"{prefix}.areasWithChloride"] = area.AreasWithChloride?.ToString();
            }
        }

        private void AddAppendixData(Dictionary<string, string> data, string prefix, AppendixDto.ObjectivesDto appendix)
        {
            if (appendix != null)
            {
                data[$"{prefix}.identified"] = appendix.Identified?.ToString();
                data[$"{prefix}.achieved"] = appendix.Achieved?.ToString();
            }
        }

        private string NormalizeRadioButtonValue(object value)
        {
            if (value == null) return string.Empty;

            if (value is bool boolValue)
            {
                return boolValue ? "true" : "false";
            }

            if (value is string stringValue)
            {
                // Assume string is already in "Yes"/"No"
                if (stringValue.Equals("Yes", System.StringComparison.OrdinalIgnoreCase) ||
                    stringValue.Equals("No", System.StringComparison.OrdinalIgnoreCase))
                {
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                    return ti.ToTitleCase(stringValue);
                }
            }

            return string.Empty; // Default to empty if value is invalid
        }
    }
}
