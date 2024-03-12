export const tooltips = {
  objectives: 'The salt management plan should include a description of planned improvements to be undertaken over time. Achievement of the objectives is an indicator of performance in the implementation of the salt management plan and will be taken into consideration in the second review of progress on the Code of Practice to be conducted by Environment Canada.',
  acetate: 'calcium-magnesium or potassium acetates',
  nonchloride: 'record only pure non-chloride (e.g. beet juice, corn bi-product, molasses or other organics), excluding pre-mixed blends with salt brine',
  liquids: 'Describe multi-chloride liquids that contain more than one type of salt in the same mix'
}

export const defaultValues = {
  reportTypeId: 4,
  serviceArea: '',
  contactName: '',
  telephone: '',
  email: '',
  sect1: {
    planDeveloped: '',
    planReviewed: '',
    planUpdated: '',
    training: {
      manager: '',
      supervisor: '',
      operator: '',
      mechanical: '',
      patroller: '',
    },
    objectives: {
      materialStorage: {
        identified: null,
        achieved: null,
      },
      saltApplication: {
        identified: null,
        achieved: null,
      },
    },
  },
  sect2: {
    roadTotalLength: null,
    saltTotalDays: null,
  },
  sect3: {
    deicer: { nacl: null, mgcl2: null, cacl2: null, acetate: null },
    treatedAbrasives: { sandStoneDust: null, nacl: null, mgcl2: null, cacl2: null },
    prewetting: { nacl: null, mgcl2: null, cacl2: null, acetate: null, nonchloride: null },
    pretreatment: { nacl: null, mgcl2: null, cacl2: null, acetate: null, nonchloride: null },
    antiicing: { nacl: null, mgcl2: null, cacl2: null, acetate: null, nonchloride: null },
    multiChlorideA: {
      litres: null,
      naclPercentage: null,
      mgcl2Percentage: null,
      cacl2Percentage: null,
    },
    multiChlorideB: {
      litres: null,
      naclPercentage: null,
      mgcl2Percentage: null,
      cacl2Percentage: null,
    },
  },
  sect4: {
    saltStorageSitesTotal: null,
    stockpiles: [
      {
        siteName: '',
        motiOwned: false,
        roadSalts: {
          stockpilesTotal: null,
          onImpermeableSurface: null,
          underPermanentRoof: null,
          underTarp: null,
        },
        treatedAbrasives: {
          stockpilesTotal: null,
          onImpermeableSurface: null,
          underPermanentRoof: null,
          underTarp: null,
        },
      },
    ],
    practices: {
      allMaterialsHandled: {
        hasPlan: false,
        numSites: null,
      },
      equipmentPreventsOverloading: {
        numSites: null,
        hasPlan: false,
      },
      wastewaterSystem: {
        numSites: null,
        hasPlan: false,
      },
      controlDiversionExternalWaters: {
        numSites: null,
        hasPlan: false,
      },
      drainageCollectionSystem: {
        numSites: null,
        hasPlan: false,
      },
      municipalSewerSystem: {
        numSites: null,
        hasPlan: false,
      },
      removalContainment: {
        numSites: null,
        hasPlan: false,
      },
      watercourse: {
        numSites: null,
        hasPlan: false,
      },
      otherDischargePoint: {
        numSites: null,
        hasPlan: false,
      },
      ongoingCleanup: {
        numSites: null,
        hasPlan: false,
      },
      riskManagementPlan: {
        numSites: null,
        hasPlan: false,
      },
    },
  },
  sect5: {
    numberOfVehicles: null,
    vehiclesForSaltApplication: null,
    vehiclesWithConveyors: null,
    vehiclesWithPreWettingEquipment: null,
    vehiclesForDLA: null,
    regularCalibration: null,
    regularCalibrationTotal: null,
    weatherMonitoringSources: {
      infraredThermometer: {
        relied: false,
        number: 0,
      },
      meteorologicalService: {
        relied: false,
      },
      fixedRWISStations: {
        relied: false,
        number: 0,
      },
      mobileRWISMounted: {
        relied: false,
        number: 0,
      },
    },
    maintenanceDecisionSupport: {
      avl: {
        relied: false,
        number: 0,
      },
      saltApplicationRates: {
        relied: false,
        number: 0,
      },
      applicationRateChart: {
        relied: false,
        number: 0,
      },
      testingMDSS: {
        relied: false,
        number: 0,
      },
    },
  },
  sect6: {
    disposal: {
      used: false,
      total: 0,
    },
    snowMelter: {
      used: false,
    },
    meltwater: {
      used: false,
    },
  },
  sect7: {
    completedInventory: '',
    setVulnerableAreas: '',
    actionPlanPrepared: '',
    protectionMeasuresImplemented: '',
    environmentalMonitoringConducted: '',
    typesOfVulnerableAreas: {
      drinkingWater: {
        areasIdentified: null,
        areasWithProtection: null,
        areasWithChloride: null,
      },
      aquaticLife: {
        areasIdentified: null,
        areasWithProtection: null,
        areasWithChloride: null,
      },
      wetlands: {
        areasIdentified: null,
        areasWithProtection: null,
        areasWithChloride: null,
      },
      delimitedAreas: {
        areasIdentified: null,
        areasWithProtection: null,
        areasWithChloride: null,
      },
      valuedLands: {
        areasIdentified: null,
        areasWithProtection: null,
        areasWithChloride: null,
      },
    },
    vulnerableAreas: [
      {
        highway: null,
        latitude: null,
        longitude: null,
        feature: null,
        type: null,
        protectionMeasures: null,
        monitoringInPlace: false,
      },
    ],
  },
  appendix: {
    materialStorage: {
      newSaltDomeWithPad: { identified: null, achieved: null },
      newSaltDomeIndoorStorage: { identified: null, achieved: null },
      upgradeSaltStorageSites: { identified: null, achieved: null },
      constructPermanentCoverStructure: { identified: null, achieved: null },
      impermeablePadForAbrasives: { identified: null, achieved: null },
      expandInsideBuildingForAbrasives: { identified: null, achieved: null },
      useTarpsForAbrasives: { identified: null, achieved: null },
      reconfigureStorageCapacity: { identified: null, achieved: null },
      reconfigureOperationFacilities: { identified: null, achieved: null },
      designAreaForTruckLoading: { identified: null, achieved: null },
      controlTruckLoading: { identified: null, achieved: null },
      installEquipmentWashBay: { identified: null, achieved: null },
      designSiteForRunoffControl: { identified: null, achieved: null },
      manageSaltContaminatedWaters: { identified: null, achieved: null },
      spillPreventionPlan: { identified: null, achieved: null },
      removeContaminatedSnow: { identified: null, achieved: null },
      otherSpecify: { identified: null, achieved: null },
    },
    saltApplication: {
      installGroundSpeedControls: { identified: null, achieved: null },
      increasePreWettingEquipment: { identified: null, achieved: null },
      installLiquidAntiIcing: { identified: null, achieved: null },
      installInfraredThermometers: { identified: null, achieved: null },
      installAdditionalRWISStations: { identified: null, achieved: null },
      accessRWISData: { identified: null, achieved: null },
      installMobileRWIS: { identified: null, achieved: null },
      accessMeteorologicalService: { identified: null, achieved: null },
      adoptPreWettingMajorityNetwork: { identified: null, achieved: null },
      usePreTreatedSalt: { identified: null, achieved: null },
      adoptPreWettingOrTreatmentAbrasives: { identified: null, achieved: null },
      testingNewProducts: { identified: null, achieved: null },
      adoptAntiIcingStandard: { identified: null, achieved: null },
      installGPSAndComputerSystems: { identified: null, achieved: null },
      useChartForApplicationRates: { identified: null, achieved: null },
      useMDSS: { identified: null, achieved: null },
      reviewSaltUse: { identified: null, achieved: null },
      assessPlowingEfficiency: { identified: null, achieved: null },
      other: { identified: null, achieved: null },
    },
    snowDisposal: {
      developProgramPhaseOut: { identified: null, achieved: null },
      installNewSiteLowPermeability: { identified: null, achieved: null },
      upgradeExistingSiteLowPermeability: { identified: null, achieved: null },
      collectMeltWaterSpecificPoint: { identified: null, achieved: null },
      constructCollectionPond: { identified: null, achieved: null },
      otherSnowDisposal: { identified: null, achieved: null },
    },
    vulnerableAreas: {
      identifySaltVulnerableAreas: { identified: null, achieved: null },
      prioritizeAreasForAdditionalProtection: { identified: null, achieved: null },
      implementProtectionMitigationMeasures: { identified: null, achieved: null },
      conductEnvironmentalMonitoring: { identified: null, achieved: null },
      otherVulnerableAreas: { identified: null, achieved: null },
    },
  },
};
